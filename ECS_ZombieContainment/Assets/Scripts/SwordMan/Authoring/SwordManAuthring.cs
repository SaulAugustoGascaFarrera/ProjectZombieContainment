using Unity.Entities;
using UnityEngine;

public class SwordManAuthring : MonoBehaviour
{

    public GameObject targetGameObject;
    public int damageAmount;
    public float movementSpeed;
    public class Baker : Baker<SwordManAuthring>
    {
        public override void Bake(SwordManAuthring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new SwordMan
            {
                targetEntity = GetEntity(authoring.targetGameObject,TransformUsageFlags.Dynamic),
                damageAmount = authoring.damageAmount,
                movementSpeed = authoring.movementSpeed
            });
        }
    }
}


public struct SwordMan : IComponentData
{
    public Entity targetEntity;
    public int damageAmount;
    public float movementSpeed;
}