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
         
        foreach((RefRW<LocalTransform> localTransform,RefRW<SwordMan> swordMan,RefRW<Target> target,Entity entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<SwordMan>,RefRW<Target>>().WithEntityAccess())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }


            //Unit unit = SystemAPI.GetComponent<Unit>(swordMan.ValueRO.targetEntity);


            //if(target.ValueRO.targetFaction != unit.faction)
            //{
            //    UnityEngine.Debug.Log("No hay un Target zombie");
            //    continue;
            //}

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            float3 moveDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;

            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Position += moveDirection * swordMan.ValueRO.movementSpeed * SystemAPI.Time.DeltaTime;

            float distanceToDestroy = 0.2f;

            

            if(math.distancesq(localTransform.ValueRO.Position,targetLocalTransform.Position) < distanceToDestroy)
            {
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);

                swordMan.ValueRW.damageAmount = targetHealth.ValueRO.healthAmount;

                targetHealth.ValueRW.healthAmount -= swordMan.ValueRO.damageAmount;

                entityCommandBuffer.DestroyEntity(entity);


            }


        }

    }

   
}
