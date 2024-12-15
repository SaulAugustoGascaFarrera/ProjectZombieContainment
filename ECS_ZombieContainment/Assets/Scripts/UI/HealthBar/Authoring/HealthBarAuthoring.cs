using Unity.Entities;
using UnityEngine;

public class HealthBarAuthoring : MonoBehaviour
{

    public GameObject barVisualEntity;
    public GameObject healthEntity;
    public class Baker : Baker<HealthBarAuthoring>
    {
        public override void Bake(HealthBarAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new HealthBar
            {
                barVisualEntity = GetEntity(authoring.barVisualEntity, TransformUsageFlags.NonUniformScale),
                healthEntity = GetEntity(authoring.healthEntity, TransformUsageFlags.Dynamic)
            });
        }
    }
}


public struct HealthBar : IComponentData
{
    public Entity barVisualEntity;
    public Entity healthEntity;
}
