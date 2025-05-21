using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SelectedVisualSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithDisabled<Selected>())
        {
            RefRW<LocalTransform> visualLocalTranasform  = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);

            visualLocalTranasform.ValueRW.Scale = 0.0f;
        }

        foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>())
        {
            RefRW<LocalTransform> visualLocalTranasform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);

            visualLocalTranasform.ValueRW.Scale = selected.ValueRO.showScale;
        }
    }

   
}
