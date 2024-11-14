using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    public GameObject entityTargetGameObeject;
    public int damageAmount;
    public float movementSpeed;

    public class Baker :Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Bullet
            {
                entityTarget = GetEntity(authoring.entityTargetGameObeject,TransformUsageFlags.Dynamic),
                damageAmount = authoring.damageAmount,
                movementSpeed = authoring.movementSpeed,
            });
        }
    }
}

public struct Bullet : IComponentData
{
    public Entity entityTarget;
    public int damageAmount;
    public float movementSpeed;
}
