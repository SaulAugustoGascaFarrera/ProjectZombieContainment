using Unity.Burst;
using Unity.Entities;
using UnityEngine;

partial struct ShotAttackSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRW<ShotAttack> shotAttack,RefRW<FindTarget> findTarget,RefRO<Target> target) in SystemAPI.Query<RefRW<ShotAttack>,RefRW<FindTarget>, RefRO<Target>>())
        {
            if(target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }

            shotAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if(shotAttack.ValueRO.timer > 0.0f)
            {
                continue;
            }

            shotAttack.ValueRW.timer = shotAttack.ValueRO.timerMax;

            Debug.Log("SHOTTINGGGG");

            RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);

            targetHealth.ValueRW.healthAmount -= 5;


        }
    }

}
