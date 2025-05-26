using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;


[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateBefore(typeof(ResetEventsSystems))]
partial struct SelectedVisualSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithPresent<Selected>())
        {
            

            if(selected.ValueRO.onSelected)
            {
               
                RefRW<LocalTransform> visualLocalTranasform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                visualLocalTranasform.ValueRW.Scale = selected.ValueRO.showScale;
            }

            if (selected.ValueRO.onDeselected)
            {
                
                RefRW<LocalTransform> visualLocalTranasform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                visualLocalTranasform.ValueRW.Scale = 0.0f;
            }


        }

       
    }

   
}
