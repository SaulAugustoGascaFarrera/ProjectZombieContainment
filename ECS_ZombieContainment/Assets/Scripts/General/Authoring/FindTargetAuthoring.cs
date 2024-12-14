using Unity.Entities;
using UnityEngine;

public class FindTargetAuthoring : MonoBehaviour
{
    public float maxTimer;
    public float range;

    public class Baker : Baker<FindTargetAuthoring>
    {
        public override void Bake(FindTargetAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new FindTarget
            {
                maxTimer = authoring.maxTimer,
                range = authoring.range
            });
        }
    }
}

public struct FindTarget : IComponentData
{
    public float timer;
    public float maxTimer;
    public float range;
}
