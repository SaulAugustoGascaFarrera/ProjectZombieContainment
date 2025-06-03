using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SetupUnitMoverDefaultPositionSystem : ISystem
{
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach((RefRO<LocalTransform> localTransform,RefRO<SetupUnitMoverDefaultPosition> setupUnitMoverDefaultPosition,RefRW<UnitMover> unitMover,Entity entity) in SystemAPI.Query<RefRO<LocalTransform>,RefRO<SetupUnitMoverDefaultPosition>, RefRW<UnitMover>>().WithEntityAccess())
        {
            unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
            entityCommandBuffer.RemoveComponent<SetupUnitMoverDefaultPosition>(entity);
        }
    }

    
}
