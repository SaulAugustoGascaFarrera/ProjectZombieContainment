using Unity.Burst;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;

partial struct CurseAttackSystem : ISystem
{
   
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach((RefRW<LocalTransform> localTransform,RefRW<CurseAttack> curseAttack,RefRO<Target> target) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<CurseAttack>,RefRO<Target>>())
        {
            if(!SystemAPI.Exists(target.ValueRO.targetEntity))
            {
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            
            if(math.distancesq(targetLocalTransform.Position,localTransform.ValueRO.Position) > 6.0f)
            {
                float3 moveDirection = targetLocalTransform.Position - localTransform.ValueRO.Position; 

                moveDirection = math.normalize(moveDirection);  

                localTransform.ValueRW.Position += moveDirection * 6.0f * SystemAPI.Time.DeltaTime;


            }
            else
            {
                localTransform.ValueRW.Position = localTransform.ValueRO.Position;

                curseAttack.ValueRW.rateToConvert += 5 * SystemAPI.Time.DeltaTime;

                if(curseAttack.ValueRO.rateToConvert > curseAttack.ValueRO.rateToConvertMax)
                {
                    curseAttack.ValueRW.rateToConvert = 0.0f;

                    entityCommandBuffer.DestroyEntity(target.ValueRO.targetEntity);



                    Entity zombieEntity = state.EntityManager.Instantiate(entitiesReferences.zombiePrefabEntity);
                    SystemAPI.SetComponent(zombieEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position + new float3(3.0f, 0.0f,3.0f)));


                    entityCommandBuffer.AddComponent(zombieEntity, new RandomWalking
                    {
                        originPosition = localTransform.ValueRO.Position,
                        targetPosition = localTransform.ValueRO.Position,
                        distanceMin = 3,
                        distanceMax = 10,
                        random = new Unity.Mathematics.Random((uint)zombieEntity.Index)
                    });

                    


                    

                }

            }


        }
       
    }

}
