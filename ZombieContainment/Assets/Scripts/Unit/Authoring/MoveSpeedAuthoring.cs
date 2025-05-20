using Unity.Entities;
using UnityEngine;

public class MoveSpeedAuthoring : MonoBehaviour
{

    public float movementSpeed;
    public float rotationSpeed;

    public class Baker : Baker<MoveSpeedAuthoring>
    {
        public override void Bake(MoveSpeedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity,new MoveSpeed
            {
                movementSpeed = authoring.movementSpeed,
                rotationSpeed = authoring.rotationSpeed,
            });
        }
    }
    
    
}

public struct MoveSpeed : IComponentData
{
    public float movementSpeed;
    public float rotationSpeed;
}
