
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct BulletMoverSystem : ISystem
{
   

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach((RefRW<LocalTransform> localTransform,RefRW<Bullet> bullet,RefRO<Target> target,Entity entity) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<Bullet>,RefRO<Target>>().WithEntityAccess())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }

            LocalTransform targetTransformPosition = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            ShootVictim targetHitPosition = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.targetEntity);
            float3 targetPosition = targetTransformPosition.TransformPoint(targetHitPosition.hitLocalPosition);


            float distanceBeforeSq = math.distancesq(targetTransformPosition.Position,localTransform.ValueRO.Position);

            float3 moveDirection = targetPosition - localTransform.ValueRO.Position;

            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Position += moveDirection * bullet.ValueRO.movementSpeed * SystemAPI.Time.DeltaTime;

            float distanceAfterSq = math.distancesq(targetTransformPosition.Position, localTransform.ValueRO.Position);

            if (distanceAfterSq > distanceBeforeSq)
            {
                localTransform.ValueRW.Position = targetTransformPosition.Position;
            }

            float distanceToDestroy = 0.2f;

            if(math.distancesq(localTransform.ValueRO.Position, targetTransformPosition.Position) < distanceToDestroy)
            {
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);

                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;

                targetHealth.ValueRW.onHealthChanged = true;

                entityCommandBuffer.DestroyEntity(entity);
            }

        }
    }

}
