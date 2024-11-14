using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct SwordManSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
         
        foreach((RefRW<LocalTransform> localTransform,RefRW<SwordMan> swordMan,Entity entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<SwordMan>>().WithEntityAccess())
        {
            if (swordMan.ValueRO.targetEntity == Entity.Null)
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(swordMan.ValueRO.targetEntity);

            float3 moveDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;

            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Position += moveDirection * swordMan.ValueRO.movementSpeed * SystemAPI.Time.DeltaTime;

            float distanceToDestroy = 0.2f;

            

            if(math.distancesq(localTransform.ValueRO.Position,targetLocalTransform.Position) < distanceToDestroy)
            {
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(swordMan.ValueRO.targetEntity);

                swordMan.ValueRW.damageAmount = targetHealth.ValueRO.healthAmount;

                targetHealth.ValueRW.healthAmount -= swordMan.ValueRO.damageAmount;

                entityCommandBuffer.DestroyEntity(entity);


            }


        }

    }

   
}
