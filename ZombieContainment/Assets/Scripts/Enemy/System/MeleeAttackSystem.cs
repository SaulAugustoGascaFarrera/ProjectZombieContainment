using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct MeleeAttackSystem : ISystem
{
   

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        NativeList<RaycastHit> raycastHitList = new NativeList<RaycastHit>(Allocator.Temp);

        foreach((RefRW<LocalTransform> localTransform,RefRW<Target> target,RefRW<MeleeAttack> meleeAttack,RefRW<UnitMover> unitMover) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<Target>,RefRW<MeleeAttack>,RefRW<UnitMover>>().WithDisabled<MoveOverride>())
        {
            if(!SystemAPI.Exists(target.ValueRO.targetEntity))
            {
                continue;
            }


            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            float meleeAttackDistanceSq = 2.0f;

            bool isCloseEnoughToAttack = math.distancesq(localTransform.ValueRO.Position,targetLocalTransform.Position) < meleeAttackDistanceSq;

            bool isTouchingTarget = false;

            if(!isCloseEnoughToAttack)
            {
                float3 directionToTarget = targetLocalTransform.Position - localTransform.ValueRO.Position;
                directionToTarget = math.normalize(directionToTarget);
                float distanceExtraToTesRaycast = 4.0f;

                RaycastInput raycastInput = new RaycastInput
                {
                    Start = localTransform.ValueRO.Position,
                    End = localTransform.ValueRO.Position + directionToTarget * (meleeAttack.ValueRO.colliderSize * distanceExtraToTesRaycast),
                    Filter = CollisionFilter.Default
                };

                raycastHitList.Clear();

                if(collisionWorld.CastRay(raycastInput,ref raycastHitList))
                {
                    foreach(RaycastHit raycastHit in raycastHitList)
                    {
                        if(raycastHit.Entity == target.ValueRO.targetEntity)
                        {
                            //raycast hit target,close enough to attack this entity
                            isTouchingTarget = true;
                            break;

                        }
                    }
                }
                   
            }

            if(!isCloseEnoughToAttack && !isTouchingTarget)
            {
                //target is too far, move close
                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
            }
            else
            {
                //target is close enough to attack
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;

                meleeAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

                if (meleeAttack.ValueRO.timer > 0.0f)
                {
                    continue;
                }

                meleeAttack.ValueRW.timer = meleeAttack.ValueRO.timerMax;

                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRW.targetEntity);
                targetHealth.ValueRW.onHealthChanged = true;
                targetHealth.ValueRW.healthAmount -= meleeAttack.ValueRO.damageAmount;

            }

            
        }
    }

    
}
