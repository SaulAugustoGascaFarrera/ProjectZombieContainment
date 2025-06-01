using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup),OrderLast = true)]
partial struct ResetEventsSystems : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<Selected> selected in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
        {
            selected.ValueRW.onSelected = false;
            selected.ValueRW.onDeselected = false;
        }

        foreach(RefRW<Health> health in SystemAPI.Query<RefRW<Health>>())
        {
            health.ValueRW.onHealthChanged = false;
        }
    }

    
}
