using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ZombieSpawnerSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach((RefRW<LocalTransform> localTransform,RefRW<ZombieSpawner> zombieSpawner) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<ZombieSpawner>>())
        {

            zombieSpawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if(zombieSpawner.ValueRO.timer > 0.0f)
            {
                continue;
            }

            zombieSpawner.ValueRW.timer = zombieSpawner.ValueRO.maxTimer;


            Entity zombieEntity = state.EntityManager.Instantiate(entitiesReferences.zombiePrefabEntity);
            SystemAPI.SetComponent(zombieEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));

        }
    }

    
}
