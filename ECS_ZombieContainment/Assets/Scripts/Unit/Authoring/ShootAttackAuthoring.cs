using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ShootAttackAuthoring : MonoBehaviour
{

    
    public float maxTimer;
    public int damageAmount;
    public Transform bulletSpawnTranform;
    public float attackDistance;
    public class Baker : Baker<ShootAttackAuthoring>
    {
        public override void Bake(ShootAttackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new ShootAttack
            {
               
                maxTimer = authoring.maxTimer,
                damageAmount = authoring.damageAmount,
                bulletSpawnLocalPosition = authoring.bulletSpawnTranform.localPosition,
                attackDistance = authoring.attackDistance,
            });
        }
    }
}

public struct ShootAttack : IComponentData
{
    public float timer;
    public float maxTimer;
    public int damageAmount;
    public float3 bulletSpawnLocalPosition;
    public float attackDistance;
}
