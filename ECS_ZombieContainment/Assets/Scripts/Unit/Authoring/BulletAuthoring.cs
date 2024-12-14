using Unity.Entities;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
   
    public int damageAmount;
    public float movementSpeed;
    

    public class Baker :Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Bullet
            {

                damageAmount = authoring.damageAmount,
                movementSpeed = authoring.movementSpeed,
                
            });
        }
    }
}

public struct Bullet : IComponentData
{
    
    public int damageAmount;
    public float movementSpeed;
}
