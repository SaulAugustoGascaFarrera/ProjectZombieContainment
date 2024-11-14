using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BulletSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach((RefRW<LocalTransform> localTransform,RefRW<Bullet> bullet, Entity entity) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<Bullet>>().WithEntityAccess())
        {

            if(bullet.ValueRO.entityTarget == Entity.Null)
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }


            RefRO<LocalTransform> targetLocalTransfom = SystemAPI.GetComponentRO<LocalTransform>(bullet.ValueRO.entityTarget);


            float3 moveDirection = targetLocalTransfom.ValueRO.Position - localTransform.ValueRO.Position;

            moveDirection = math.normalize(moveDirection);

            float distanceBeforeSq = math.distancesq(localTransform.ValueRO.Position,targetLocalTransfom.ValueRO.Position);

            localTransform.ValueRW.Position += moveDirection * bullet.ValueRO.movementSpeed * SystemAPI.Time.DeltaTime;


            float distanceAfterSq = math.distancesq(localTransform.ValueRO.Position, targetLocalTransfom.ValueRO.Position);

            if(distanceAfterSq > distanceBeforeSq)
            {
                localTransform.ValueRW.Position = targetLocalTransfom.ValueRO.Position;
            }

            float distanceToDestroy = 0.2f;

            if (math.distancesq(localTransform.ValueRO.Position,targetLocalTransfom.ValueRO.Position) < distanceToDestroy)
            {
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(bullet.ValueRO.entityTarget);

                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;

                entityCommandBuffer.DestroyEntity(entity);
            }

           
        }
    }

    
}
