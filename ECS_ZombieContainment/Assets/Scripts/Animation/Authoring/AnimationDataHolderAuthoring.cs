using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class AnimationDataHolderAuthoring : MonoBehaviour
{

    //public AnimationDataSO soldierIdle;
    //public AnimationDataSO soldierWalk;
    public AnimationDataListSO animationDataListSO;
   public class Baker : Baker<AnimationDataHolderAuthoring>
   {
        public override void Bake(AnimationDataHolderAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AnimationDataHolder animationDataHolder = new AnimationDataHolder();

            EntitiesGraphicsSystem entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();


            BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
            ref BlobArray<AnimationData> animationDataBlobArray = ref blobBuilder.ConstructRoot<BlobArray<AnimationData>>();

            //BlobBuilderArray<AnimationData> animationDataBlobBuilderArray = blobBuilder.Allocate<AnimationData>(ref animationDataBlobArray, 2);

            BlobBuilderArray<AnimationData> animationDataBlobBuilderArray = blobBuilder.Allocate<AnimationData>(ref animationDataBlobArray, System.Enum.GetValues(typeof(AnimationDataSO.AnimationType)).Length);

            int index = 0;

            foreach(AnimationDataSO.AnimationType animationType in System.Enum.GetValues(typeof(AnimationDataSO.AnimationType)))
            {
                AnimationDataSO animationDataSO = authoring.animationDataListSO.GetAnimationDataSO(animationType);

                BlobBuilderArray<BatchMeshID> bloBuilderArray = blobBuilder.Allocate<BatchMeshID>(ref animationDataBlobBuilderArray[index].batchMeshIDBlobArray, animationDataSO.meshArray.Length);
                animationDataBlobBuilderArray[index].frameTimerMax = animationDataSO.frameTimerMax;
                animationDataBlobBuilderArray[index].frameMax = animationDataSO.meshArray.Length;

                for (int i = 0; i < animationDataSO.meshArray.Length; i++)
                {
                            Mesh mesh = animationDataSO.meshArray[i];
                            bloBuilderArray[i] = entitiesGraphicsSystem.RegisterMesh(mesh);
                }

                index++;
            }

            //{


            //    BlobBuilderArray<BatchMeshID> bloBuilderArray = blobBuilder.Allocate<BatchMeshID>(ref animationDataBlobBuilderArray[0].batchMeshIDBlobArray, authoring.soldierIdle.meshArray.Length);
            //    animationDataBlobBuilderArray[0].frameTimerMax = authoring.soldierIdle.frameTimerMax;
            //    animationDataBlobBuilderArray[0].frameMax = authoring.soldierIdle.meshArray.Length;

            //    for (int i = 0; i < authoring.soldierIdle.meshArray.Length; i++)
            //    {
            //            Mesh mesh = authoring.soldierIdle.meshArray[i];
            //            bloBuilderArray[i] = entitiesGraphicsSystem.RegisterMesh(mesh);
            //    }
            //}


            //{


            //    BlobBuilderArray<BatchMeshID> bloBuilderArray = blobBuilder.Allocate<BatchMeshID>(ref animationDataBlobBuilderArray[1].batchMeshIDBlobArray, authoring.soldierWalk.meshArray.Length);
            //    animationDataBlobBuilderArray[1].frameTimerMax = authoring.soldierIdle.frameTimerMax;
            //    animationDataBlobBuilderArray[1].frameMax = authoring.soldierIdle.meshArray.Length;

            //    for (int i = 0; i < authoring.soldierWalk.meshArray.Length; i++)
            //    {
            //        Mesh mesh = authoring.soldierWalk.meshArray[i];
            //        bloBuilderArray[i] = entitiesGraphicsSystem.RegisterMesh(mesh);
            //    }
            //}

            animationDataHolder.animationDataBlobArrayBlobAssetReference = blobBuilder.CreateBlobAssetReference<BlobArray<AnimationData>>(Allocator.Persistent);

            blobBuilder.Dispose();


            AddBlobAsset(ref animationDataHolder.animationDataBlobArrayBlobAssetReference, out Unity.Entities.Hash128 objectHash);
            

            //{
            //    BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
            //    ref AnimationData animationData = ref blobBuilder.ConstructRoot<AnimationData>();
            //    animationData.frameTimerMax = authoring.soldierWalk.frameTimerMax;
            //    animationData.frameMax = authoring.soldierWalk.meshArray.Length;

            //    BlobBuilderArray<BatchMeshID> bloBuilderArray = blobBuilder.Allocate<BatchMeshID>(ref animationData.batchMeshIDBlobArray, authoring.soldierWalk.meshArray.Length);


            //    for (int i = 0; i < authoring.soldierWalk.meshArray.Length; i++)
            //    {
            //        Mesh mesh = authoring.soldierWalk.meshArray[i];
            //        bloBuilderArray[i] = entitiesGraphicsSystem.RegisterMesh(mesh);
            //    }

            //    animationDataHolder.soldierWalk = blobBuilder.CreateBlobAssetReference<AnimationData>(Allocator.Persistent);

            //    blobBuilder.Dispose();


            //    AddBlobAsset(ref animationDataHolder.soldierWalk, out Unity.Entities.Hash128 objectHash);
            //}

            AddComponent(entity, animationDataHolder);
        }
   }
}


public struct AnimationDataHolder : IComponentData
{
    public BlobAssetReference<BlobArray<AnimationData>> animationDataBlobArrayBlobAssetReference;
    //public BlobAssetReference<AnimationData> soldierWalk;
}

public struct AnimationData
{
    public float frameTimerMax;
    public int frameMax;
    public BlobArray<BatchMeshID> batchMeshIDBlobArray;
}
