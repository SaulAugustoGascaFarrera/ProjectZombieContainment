using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SetupUnitMoverDefaultPositionSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach((RefRW<UnitMover> unitMover,RefRO<LocalTransform> localTransform,RefRO<SetupUnitMoverDefaultPosition> SetupUnitMoverDefaultPosition,Entity entity) in SystemAPI.Query<RefRW<UnitMover>,RefRO<LocalTransform>,RefRO <SetupUnitMoverDefaultPosition>>().WithEntityAccess())
        {
             unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;


            entityCommandBuffer.RemoveComponent<SetupUnitMoverDefaultPosition>(entity);
        }
    }

   
}
