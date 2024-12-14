using System.Reflection.Emit;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MeleeAttackSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<LocalTransform> localTransform, RefRW<MeleeAttack> meleeAttack, RefRO<Target> target, RefRW<UnitMover> unitMover) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MeleeAttack>, RefRO<Target>, RefRW<UnitMover>>().WithDisabled<MoveOverride>())
        {
            if (!SystemAPI.Exists(target.ValueRO.targetEntity))
            {
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            float distanceToAttack = 2.0f;

            if (math.distancesq(targetLocalTransform.Position, localTransform.ValueRO.Position) > distanceToAttack)
            {
                //float3 moveToTargetDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;

                //moveToTargetDirection = math.normalize(moveToTargetDirection);

                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;

            }
            else
            {

                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;

                meleeAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

                if (meleeAttack.ValueRO.timer > 0.0f)
                {
                    continue;
                }

                meleeAttack.ValueRW.timer = meleeAttack.ValueRO.timerMax;


                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= meleeAttack.ValueRO.damageAmount;

            }


        }
    }

   
}
