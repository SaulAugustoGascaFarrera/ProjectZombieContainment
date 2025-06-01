using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

partial struct UnitMoverSystem : ISystem
{

    public const float REACHED_TARGET_POSITION_DISTANCE_SQ = 2.0f;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UnitMoverJob unitMoverJob = new UnitMoverJob{
            deltaTime = SystemAPI.Time.DeltaTime
        };


        unitMoverJob.ScheduleParallel();

    }

}


[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{

    public float deltaTime;
    public void Execute(ref LocalTransform localTransform,in UnitMover unitMover,ref PhysicsVelocity physicsVelocity)
    {
        float3 moveDirection = unitMover.targetPosition - localTransform.Position;


        

        if(math.lengthsq(moveDirection) > UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQ)
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
        }

        
    }
}
