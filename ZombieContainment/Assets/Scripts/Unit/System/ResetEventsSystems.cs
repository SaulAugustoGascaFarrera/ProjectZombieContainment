using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup),OrderLast = true)]
partial struct ResetEventsSystems : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        new ResetSelectedEventsJob().ScheduleParallel();
        new ResetHealthEventsJob().ScheduleParallel();

        //foreach (RefRW<Selected> selected in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
        //{
        //    selected.ValueRW.onSelected = false;
        //    selected.ValueRW.onDeselected = false;
        //}

        //foreach(RefRW<Health> health in SystemAPI.Query<RefRW<Health>>())
        //{
        //    health.ValueRW.onHealthChanged = false;
        //}
    }

    
}

[BurstCompile]
public partial struct ResetHealthEventsJob : IJobEntity
{
    public void Execute(ref Health health)
    {
        health.onHealthChanged = false;
    }
}


[BurstCompile]
[WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
public partial struct ResetSelectedEventsJob : IJobEntity
{
    public void Execute(ref Selected selected)
    {
        selected.onSelected = false;
        selected.onDeselected = false;
    }
}
