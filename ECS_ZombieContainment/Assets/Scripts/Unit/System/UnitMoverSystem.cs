using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct UnitMoverSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

       

            UnitMoverJob unitMoverJob = new UnitMoverJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                //reachedTargetDistanceSqr = 2.0f
            };

            unitMoverJob.ScheduleParallel();
        

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}


[BurstCompile]  
public partial struct UnitMoverJob : IJobEntity
{
    public float deltaTime;

    

    public void Execute(ref LocalTransform localTransform,in UnitMover unitMover,ref PhysicsVelocity physicsVelocity)
    {
        float3 moveDirection = unitMover.targetPosition - localTransform.Position;

        float reachedTargetDistanceSqr = 2.0f;

        if (math.lengthsq(moveDirection) > reachedTargetDistanceSqr)
        {
            moveDirection = math.normalize(moveDirection);

            localTransform.Rotation = math.slerp(localTransform.Rotation, quaternion.LookRotation(moveDirection, math.up()), unitMover.rotationSpeed * deltaTime);

            physicsVelocity.Linear = moveDirection * unitMover.movementSpeed;

            physicsVelocity.Angular = float3.zero;
        }
        else
        {
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;
        }

       

    }

}
