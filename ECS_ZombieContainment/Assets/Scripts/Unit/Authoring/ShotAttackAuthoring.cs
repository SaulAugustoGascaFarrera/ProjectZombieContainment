using Unity.Entities;
using UnityEngine;

public class ShotAttackAuthoring : MonoBehaviour
{

   
    public float timerMax;
    public class Baker : Baker<ShotAttackAuthoring>
    {
        public override void Bake(ShotAttackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new ShotAttack
            {
                timerMax = authoring.timerMax,
            });
        }
    }
}

public struct ShotAttack : IComponentData
{
    public float timer;
    public float timerMax;
}
