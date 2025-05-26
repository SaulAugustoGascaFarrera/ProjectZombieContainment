using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ShootAttackSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach((RefRO<LocalTransform> localTransform,RefRO<Target> target,RefRW<ShootAttack> shootAttack) in SystemAPI.Query<RefRO<LocalTransform>,RefRO<Target>,RefRW<ShootAttack>>())
        {
            if(target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }

            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if(shootAttack.ValueRO.timer > 0.0f)
            {
                continue;
            }

            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;


            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletEntity);
            SystemAPI.SetComponent(bulletEntity,LocalTransform.FromPosition(localTransform.ValueRO.Position));

            //RefRW<Bullet> bulletPrefab = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            //bulletPrefab.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;
        }
    }

}
