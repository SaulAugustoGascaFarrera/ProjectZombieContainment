using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct FindTargetSystem : ISystem
{
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();


        NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);

        foreach ((RefRW<LocalTransform> localTransform, RefRW<FindTarget> findTarget, RefRW<Target> target, RefRO<Unit> unit) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<FindTarget>, RefRW<Target>, RefRO<Unit>>())
        {

            //if(unit.ValueRO.faction != FactionType.Friendly)
            //{
            //    continue;
            //}


            distanceHitList.Clear();

            CollisionFilter filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << 8 | 1u << 9,
                GroupIndex = 0
            };

            if (physicsWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.range, ref distanceHitList, filter))
            {

                foreach (DistanceHit distanceHit in distanceHitList)
                {

                    if(!SystemAPI.Exists(distanceHit.Entity) || !SystemAPI.HasComponent<Faction>(distanceHit.Entity))
                    {
                        continue;
                    }

                    Faction faction = SystemAPI.GetComponent<Faction>(distanceHit.Entity);

                    target.ValueRW.targetEntity = distanceHit.Entity;
                }
            }
        }
    }

    
}
