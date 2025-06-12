using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;


[UpdateInGroup(typeof(SimulationSystemGroup),OrderFirst = true)]
partial struct ResetTargetSystem : ISystem
{
    private ComponentLookup<LocalTransform> localTransformComponentLookUp;
    private EntityStorageInfoLookup entityStorageInfoLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        localTransformComponentLookUp = state.GetComponentLookup<LocalTransform>(true);
        entityStorageInfoLookup = state.GetEntityStorageInfoLookup();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        //localTransformComponentLookUp.Update(ref state);
        //entityStorageInfoLookup.Update(ref state);

        //ResetTargetJob resetTargetJob = new ResetTargetJob{
        //    localTransformComponentLookUp = localTransformComponentLookUp,
        //    entityStorageInfoLookup = entityStorageInfoLookup
        //};


        //ResetTargetOverrideJob resetTargetOverrideJob = new ResetTargetOverrideJob
        //{
        //    localTransformComponentLookUp = localTransformComponentLookUp,
        //    entityStorageInfoLookup = entityStorageInfoLookup
        //};

        //resetTargetJob.ScheduleParallel();

        //resetTargetOverrideJob.ScheduleParallel();


        foreach (RefRW<Target> target in SystemAPI.Query<RefRW<Target>>())
        {
            if (target.ValueRW.targetEntity != Entity.Null)
            {
                if (!SystemAPI.Exists(target.ValueRO.targetEntity) || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity))
                {
                    target.ValueRW.targetEntity = Entity.Null;
                }
            }

        }

        foreach (RefRW<TargetOverride> targetOverride in SystemAPI.Query<RefRW<TargetOverride>>())
        {
            if (targetOverride.ValueRW.targetEntity != Entity.Null)
            {
                if (!SystemAPI.Exists(targetOverride.ValueRO.targetEntity) || !SystemAPI.HasComponent<LocalTransform>(targetOverride.ValueRO.targetEntity))
                {
                    targetOverride.ValueRW.targetEntity = Entity.Null;
                }
            }

        }
    }

    
}


[BurstCompile]
public partial struct ResetTargetJob : IJobEntity
{
    [ReadOnly(true)] public ComponentLookup<LocalTransform> localTransformComponentLookUp;
    public EntityStorageInfoLookup entityStorageInfoLookup;

    public void Execute(ref Target target)
    {
        if(target.targetEntity != Entity.Null)
        {
            if(!localTransformComponentLookUp.HasComponent(target.targetEntity) || !entityStorageInfoLookup.Exists(target.targetEntity))
            {
                target.targetEntity = Entity.Null;
            }
        }
    }
}


[BurstCompile]
public partial struct ResetTargetOverrideJob : IJobEntity
{

    [ReadOnly(true)] public ComponentLookup<LocalTransform> localTransformComponentLookUp;
     public EntityStorageInfoLookup entityStorageInfoLookup;
    public void Execute(ref TargetOverride targetOverride)
    {
        if(targetOverride.targetEntity != Entity.Null)
        {
            if (!localTransformComponentLookUp.HasComponent(targetOverride.targetEntity) || !entityStorageInfoLookup.Exists(targetOverride.targetEntity))
            {
                targetOverride.targetEntity = Entity.Null;
            }
        }
    }
}
