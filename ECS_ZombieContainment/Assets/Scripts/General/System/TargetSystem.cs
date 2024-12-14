using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct TargetSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    { 
    //{
    //    EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
    //    EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

    //    foreach((RefRW<LocalTransform> localTransform,RefRO<Target> target,RefRW<ShootAttack> shootAttack ,Entity entity) in SystemAPI.Query<RefRW<LocalTransform>,RefRO<Target>,RefRW<ShootAttack>>().WithEntityAccess())
    //    {
    //        if(!SystemAPI.Exists(target.ValueRO.targetEntity))
    //        {   
    //            entityCommandBuffer.DestroyEntity(entity);
    //            continue;
    //        }

    //        LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);


    //        float3 moveDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;

    //        moveDirection = math.normalize(moveDirection);

    //        localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation,quaternion.LookRotation(moveDirection,math.up()),7.0f * SystemAPI.Time.DeltaTime);


    //        shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

    //        if (shootAttack.ValueRO.timer > 0.0f)
    //        {
    //            continue;
    //        }

    //        shootAttack.ValueRW.timer = shootAttack.ValueRO.maxTimer;

    //        Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletPrefabEntity);
    //        float3 bulletSpawnWorldPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnLocalPosition);
    //        SystemAPI.SetComponent(bulletEntity,LocalTransform.FromPosition(bulletSpawnWorldPosition));



    //        RefRW<Bullet> bullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
    //        bullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;

    //        RefRW<Target> targetBullet = SystemAPI.GetComponentRW<Target>(bulletEntity);
    //        targetBullet.ValueRW.targetEntity = target.ValueRO.targetEntity;
            
    //    }
    }

   
}
