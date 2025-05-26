using Unity.Entities;
using UnityEngine;

public class ShootAttackAuthoring : MonoBehaviour
{
    public int damageAmount;
    public float timerMax;
    public class Baker : Baker<ShootAttackAuthoring>
    {
        public override void Bake(ShootAttackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new ShootAttack
            { 
                damageAmount = authoring.damageAmount,
                timerMax = authoring.timerMax
            });
        }
    }
}


public struct ShootAttack : IComponentData
{
    public int damageAmount;
    public float timer;
    public float timerMax;
}
