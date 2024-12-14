using Unity.Entities;
using UnityEngine;

public class CurseAttackAuthoring : MonoBehaviour
{
    public float rateToConvertMax;

    public class Baker : Baker<CurseAttackAuthoring>
    {
        public override void Bake(CurseAttackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new CurseAttack
            {
                rateToConvertMax = authoring.rateToConvertMax,
            });
        }
    }

}


public struct CurseAttack : IComponentData
{
    public float rateToConvert;
    public float rateToConvertMax;

}