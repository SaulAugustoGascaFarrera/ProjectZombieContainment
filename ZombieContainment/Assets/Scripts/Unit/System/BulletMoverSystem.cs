using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;

partial struct BulletMoverSystem : ISystem
{
   

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach((RefRW<LocalTransform> localTransform,RefRW<Bullet> bullet,RefRO<Target> target,Entity entity) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<Bullet>,RefRO<Target>>().WithEntityAccess())
        {

            if(!SystemAPI.Exists(target.ValueRO.targetEntity))
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }

            LocalTransform targetLocalTranform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            ShootVictim targetShootVictim = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.targetEntity);

            float3 targetPosition = targetLocalTranform.TransformPoint(targetShootVictim.hitLocalPosition);

            float distanceBeforeSq = math.distancesq(localTransform.ValueRO.Position,targetPosition);


            float3 moveDirection = targetPosition - localTransform.ValueRO.Position;

            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Position += moveDirection * bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;

            float distanceAfterSq = math.distancesq(localTransform.ValueRO.Position, targetPosition);

            if(distanceAfterSq > distanceBeforeSq)
            {
                localTransform.ValueRW.Position = targetLocalTranform.Position;
            }

            float destroyDistanceSq = 0.2f;
            
            if(math.distancesq(localTransform.ValueRO.Position,targetLocalTranform.Position) < destroyDistanceSq)
            {
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.onHealthChanged = true;
                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;
                entityCommandBuffer.DestroyEntity(entity);
            }


        }
    }

    
}
