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

        foreach((RefRW<LocalTransform> localTransform,RefRW<Bullet> bullet,RefRW<Target> target,Entity entity) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<Bullet>, RefRW<Target>>().WithEntityAccess())
        {

            if (target.ValueRO.targetEntity == Entity.Null)
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }


            RefRO<LocalTransform> targetLocalTransfom = SystemAPI.GetComponentRO<LocalTransform>(target.ValueRO.targetEntity);


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
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);

                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;

                entityCommandBuffer.DestroyEntity(entity);
            }

           
        }
    }

    
}
