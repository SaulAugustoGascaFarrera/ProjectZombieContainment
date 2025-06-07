using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct FindTargetSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton  physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);

        foreach((RefRO<LocalTransform> localTransform,RefRW<FindTarget> findTarget,RefRW<Target> target,RefRO<TargetOverride> targetOverride) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<FindTarget>,RefRW<Target>,RefRO<TargetOverride>>())
        {

            findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if(findTarget.ValueRO.timer > 0.0f)
            {
                //timer not elapsed
                continue;
            }

            findTarget.ValueRW.timer = findTarget.ValueRO.timerMax;


            if(targetOverride.ValueRO.targetEntity != Entity.Null)
            {
                target.ValueRW.targetEntity = targetOverride.ValueRO.targetEntity;
                continue;
            }


            distanceHitList.Clear();

            CollisionFilter collisionFilter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1 << GameAssets.UNIT_LAYERS,
                GroupIndex = 0
            };

            Entity closestTargetEntity = Entity.Null;

            float closestTargetDistance= float.MaxValue;

            float currentTaregtDistanceOffset = 0.0f; 

            if(target.ValueRO.targetEntity != Entity.Null)
            {
                closestTargetEntity = target.ValueRO.targetEntity;

                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

                closestTargetDistance = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);

                currentTaregtDistanceOffset = 2f;
            }


            if(collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.range,ref distanceHitList,collisionFilter))
            {
                foreach(DistanceHit distanceHit in distanceHitList)
                {

                    if(!SystemAPI.Exists(distanceHit.Entity) || !SystemAPI.HasComponent<Unit>(distanceHit.Entity))
                    {
                        continue;
                    }

                    Unit targetUnit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);

                    if(targetUnit.faction == findTarget.ValueRO.targetFaction)
                    {
                        //valid target
                        if(closestTargetEntity == Entity.Null )
                        {
                           
                            closestTargetEntity = distanceHit.Entity;
                            closestTargetDistance = distanceHit.Distance;
                        }
                        else
                        {
                            if(distanceHit.Distance + currentTaregtDistanceOffset < closestTargetDistance)
                            {
                                closestTargetEntity = distanceHit.Entity;
                                closestTargetDistance = distanceHit.Distance;
                            }
                        }

                       

                    }
                }
            }

            if(closestTargetEntity != Entity.Null)
            {
                target.ValueRW.targetEntity = closestTargetEntity;
            }
           

        }
    }

   
}
