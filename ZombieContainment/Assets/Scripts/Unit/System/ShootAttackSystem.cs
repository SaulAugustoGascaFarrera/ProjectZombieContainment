using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ShootAttackSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach((RefRW<LocalTransform> localTransform,RefRO<Target> target,RefRW<ShootAttack> shootAttack,RefRW<UnitMover> unitMover,Entity entity) in SystemAPI.Query<RefRW<LocalTransform>,RefRO<Target>,RefRW<ShootAttack>,RefRW<UnitMover>>().WithDisabled<MoveOverride>().WithEntityAccess())
        {

           

            if (target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            if (math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position) > shootAttack.ValueRO.attackDistance)
            {
                //too far ,move closer
                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
                continue;
            }
            else
            {
                //close enough, stop moving and attack

                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;


            }

            float3 aimDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;

            aimDirection = math.normalize(aimDirection);

            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, quaternion.LookRotation(aimDirection, math.up()), unitMover.ValueRO.rotationSpeed * SystemAPI.Time.DeltaTime);

            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if(shootAttack.ValueRO.timer > 0.0f)
            {
                continue;
            }

            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;

         
            RefRW<TargetOverride> enemyTargetEntity = SystemAPI.GetComponentRW<TargetOverride>(target.ValueRO.targetEntity);
            if(enemyTargetEntity.ValueRO.targetEntity == Entity.Null)
            {
                enemyTargetEntity.ValueRW.targetEntity = entity;
            }

            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletEntity);
            float3 bulletSpawnWorldPosition = localTransform.ValueRW.TransformPoint(shootAttack.ValueRO.bulletSpawnLocalPosition);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletSpawnWorldPosition));

            //RefRW<Bullet> bulletPrefab = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            //bulletPrefab.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;


        }
    }

}
