using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

partial struct UnitMoverSystem : ISystem
{
    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

          

        foreach((RefRW<LocalTransform> localTransform,RefRO<MoveSpeed> moveSpeed,RefRW<PhysicsVelocity> physicsVelocity) in SystemAPI.Query<RefRW<LocalTransform>,RefRO<MoveSpeed>, RefRW<PhysicsVelocity>>())
        {
            float3 targetPosition = MouseWorldPosition.Instance.GetPosition();

            float3 moveDirection = targetPosition - localTransform.ValueRO.Position;

            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, quaternion.LookRotation(moveDirection, math.up()), moveSpeed.ValueRO.rotationSpeed * SystemAPI.Time.DeltaTime);

            physicsVelocity.ValueRW.Linear = moveDirection * moveSpeed.ValueRO.movementSpeed;

            physicsVelocity.ValueRW.Angular = float3.zero;

            //localTransform.ValueRW.Position += moveDirection * moveSpeed.ValueRO.value * SystemAPI.Time.DeltaTime;
        }
    }

}
