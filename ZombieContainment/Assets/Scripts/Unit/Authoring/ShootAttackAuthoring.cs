using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ShootAttackAuthoring : MonoBehaviour
{
    public int damageAmount;
    public float timerMax;
    public float attackDistance;
    public Transform bulletSpawnPositionTransform;
    public class Baker : Baker<ShootAttackAuthoring>
    {
        public override void Bake(ShootAttackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new ShootAttack
            { 
                damageAmount = authoring.damageAmount,
                timerMax = authoring.timerMax,
                attackDistance = authoring.attackDistance,
                bulletSpawnLocalPosition = authoring.bulletSpawnPositionTransform.localPosition
            });
        }
    }
}


public struct ShootAttack : IComponentData
{
    public int damageAmount;
    public float timer;
    public float timerMax;
    public float attackDistance;
    public float3 bulletSpawnLocalPosition;
}
