using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ShootAttackSystem : ISystem
{
   
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach((RefRW<LocalTransform> localTransform,RefRW<ShootAttack> shootAttack,RefRW<Target> target,RefRW<UnitMover> unitMover) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<ShootAttack>,RefRW<Target>,RefRW<UnitMover>>().WithDisabled<MoveOverride>())
        {
            if(!SystemAPI.Exists(target.ValueRO.targetEntity) || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity))
            {
                continue;
            }

            LocalTransform targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            if(math.distance(targetTransform.Position,localTransform.ValueRO.Position) > shootAttack.ValueRO.attackDistance)
            {
                //too far ,move close
                unitMover.ValueRW.targetPosition = targetTransform.Position;
                continue;
            }
            else
            {
                //close enough, stop moving and attack
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
            }


            float3 aimDirection = targetTransform.Position - localTransform.ValueRO.Position;

            aimDirection = math.normalize(aimDirection);


            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, quaternion.LookRotation(aimDirection, math.up()), unitMover.ValueRO.rotationSpeed * SystemAPI.Time.DeltaTime);

            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if(shootAttack.ValueRO.timer > 0.0f)
            {
                continue ;
            }

            shootAttack.ValueRW.timer = shootAttack.ValueRO.maxTimer;

            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletPrefabEntity);
            float3 spawnBulletWorldLocation = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnLocalPosition);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(spawnBulletWorldLocation));

            RefRW<Bullet> bullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;

        }
    }  
}
