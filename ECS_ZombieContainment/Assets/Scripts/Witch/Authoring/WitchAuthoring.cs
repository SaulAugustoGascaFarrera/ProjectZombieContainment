using Unity.Entities;
using UnityEngine;

public class WitchAuthoring : MonoBehaviour
{
    public class Baker : Baker<WitchAuthoring>
    {
        public override void Bake(WitchAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Witch { });
        }
    }
}

public struct Witch : IComponentData
{

}
