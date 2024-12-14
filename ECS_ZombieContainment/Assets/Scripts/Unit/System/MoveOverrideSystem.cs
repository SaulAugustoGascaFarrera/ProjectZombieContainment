using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MoveOverrideSystem : ISystem
{
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRW<UnitMover> unitMover,RefRW<MoveOverride> moveOverride,EnabledRefRW<MoveOverride> enabledMoveOverride,RefRO<LocalTransform> localTransform) in SystemAPI.Query<RefRW<UnitMover>,RefRW<MoveOverride>,EnabledRefRW<MoveOverride>, RefRO<LocalTransform>>())
        {
           

            if(math.distancesq(localTransform.ValueRO.Position,moveOverride.ValueRO.targetPosition) > 2.0f)
            {
                unitMover.ValueRW.targetPosition = moveOverride.ValueRO.targetPosition;
            }
            else
            {
                enabledMoveOverride.ValueRW = false;
            }
        }
    }

}
