using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ZombieSpawnerAuthoring : MonoBehaviour
{
    
    public float maxTimer;

    public class Baker : Baker<ZombieSpawnerAuthoring>
    {
        public override void Bake(ZombieSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new ZombieSpawner
            {
                maxTimer = authoring.maxTimer,  
            });
        }
    }

}

public struct ZombieSpawner : IComponentData
{
    public float timer;
    public float maxTimer;
}
