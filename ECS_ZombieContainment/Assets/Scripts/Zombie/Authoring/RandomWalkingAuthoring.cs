using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RandomWalkingAuthoring : MonoBehaviour
{

    public float3 originPosition;
    public float3 targetPosition;
    public float distanceMin;
    public float distanceMax;
    public uint randomSeed;
    public class Baker : Baker<RandomWalkingAuthoring>
    {
        public override void Bake(RandomWalkingAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new RandomWalking
            {
                originPosition = authoring.originPosition,
                targetPosition = authoring.targetPosition, 
                distanceMin = authoring.distanceMin,
                distanceMax = authoring.distanceMax,
                random = new Unity.Mathematics.Random(authoring.randomSeed)
            });
        }
    }
}

public struct RandomWalking : IComponentData
{
    public float3 originPosition;
    public float3 targetPosition;
    public float distanceMin;
    public float distanceMax;
    public Unity.Mathematics.Random random;
}