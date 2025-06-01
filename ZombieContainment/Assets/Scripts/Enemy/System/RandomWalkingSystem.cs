using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct RandomWalkingSystem : ISystem
{
   

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRW<LocalTransform> localTransform, RefRW<RandomWalking> randomWalking,RefRW<UnitMover> unitMover) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<RandomWalking>,RefRW<UnitMover>>())
        {
            if(math.distancesq(localTransform.ValueRO.Position,randomWalking.ValueRO.targetPosition) < UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQ)
            {
                //recahed target distance
                Random random = randomWalking.ValueRO.random;

                float3 randomDirection = new float3(random.NextFloat(-1f,1f),0.0f,random.NextFloat(-1f,1f));
                randomDirection = math.normalize(randomDirection);

                randomWalking.ValueRW.targetPosition = randomWalking.ValueRO.originPosition + randomDirection * random.NextFloat(randomWalking.ValueRO.distanceMin,randomWalking.ValueRO.distanceMax);

                randomWalking.ValueRW.random = random;

                //UnityEngine.Debug.Log(randomWalking.ValueRO.targetPosition);
            }
            else
            {
                //too far,move closer
                unitMover.ValueRW.targetPosition = randomWalking.ValueRW.targetPosition;
            }
        }
        
    }

    
}
