using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class UnitMoverAuthoring : MonoBehaviour
{

    public float movementSpeed;
    public float rotationSpeed;

    public class Baker : Baker<UnitMoverAuthoring>
    {
        public override void Bake(UnitMoverAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity,new UnitMover
            {
                movementSpeed = authoring.movementSpeed,
                rotationSpeed = authoring.rotationSpeed,
            });
        }
    }
    
    
}

public struct UnitMover : IComponentData
{
    public float movementSpeed;
    public float rotationSpeed;
    public float3 targetPosition;
}
