using Unity.Entities;
using UnityEngine;

public class FindTargetAuthoring : MonoBehaviour
{

    public Faction targetFaction;
    public float range;

    [Header("Timers to update the find target system")]
    public float timerMax;
    public class Baker : Baker<FindTargetAuthoring>
    {
        public override void Bake(FindTargetAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new FindTarget
            {
                targetFaction = authoring.targetFaction,
                range = authoring.range,
                timerMax = authoring.timerMax
            });
        }
    }
}

public struct FindTarget : IComponentData
{
    public Faction targetFaction;
    public float range;
    public float timer;
    public float timerMax;
}
