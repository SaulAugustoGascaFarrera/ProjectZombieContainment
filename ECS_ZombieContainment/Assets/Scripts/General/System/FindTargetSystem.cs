using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

partial struct FindTargetSystem : ISystem
{
   

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        NativeList<DistanceHit> distanceHitsList = new NativeList<DistanceHit>(Allocator.Temp);

        //int unitCount = 0;
        foreach ((RefRW<LocalTransform> localTransform,RefRW<FindTarget> findTarget,RefRW<Target> target) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<FindTarget>,RefRW<Target>>())
        {

            findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if(findTarget.ValueRO.timer > 0.0f)
            {
                continue;
            }

            findTarget.ValueRW.timer = findTarget.ValueRO.timerMax;


            distanceHitsList.Clear();

            CollisionFilter collisionFilter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << 7,
                GroupIndex = 0
            };

            if(collisionWorld.OverlapSphere(localTransform.ValueRO.Position,findTarget.ValueRO.range,ref distanceHitsList,collisionFilter))
            {

                //Debug.Log(distanceHitsList.Length);


                foreach (DistanceHit distanceHit in distanceHitsList)
                {
                   

                    Unit unit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);

                    if (unit.faction == findTarget.ValueRO.targetFaction)
                    {
                        
                        target.ValueRW.targetEntity = distanceHit.Entity;
                    }

                    

                   
                }

            }

        }

      
      

    }

   
}
