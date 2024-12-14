using Unity.Entities;
using Unity.Rendering;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Rendering;

public class ActiveAnimationAuthoring : MonoBehaviour
{

    //public Mesh frame0;
    //public Mesh frame1;
    //public Mesh frame2;
    //public int frameMax;
    //public float frameTimerMax;
    public AnimationDataSO unitIdle;
    public class Baker : Baker<ActiveAnimationAuthoring>
    {
        public override void Bake(ActiveAnimationAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            EntitiesGraphicsSystem entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();

            AddComponent(entity, new ActiveAnimation
            {
                frame0 = entitiesGraphicsSystem.RegisterMesh(authoring.unitIdle.meshArray[0]),
                frame1 = entitiesGraphicsSystem.RegisterMesh(authoring.unitIdle.meshArray[1]),
                //frame2 = entitiesGraphicsSystem.RegisterMesh(authoring.unitIdle.meshArray[2]),
                frameMax = authoring.unitIdle.meshArray.Length,
                frameTimerMax = authoring.unitIdle.frameTimerMax

            });
        }
    }
}

public struct ActiveAnimation : IComponentData
{
    public int frame;
    public int frameMax;
    public float frameTimer;
    public float frameTimerMax;
    public BatchMeshID frame0;
    public BatchMeshID frame1;
    public BatchMeshID frame2;
}
