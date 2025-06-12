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

            EntitiesGraphicsSystem entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();

            AnimationDataHolder animationDataHolder = new AnimationDataHolder();

            //{
            //    BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);

            //    ref AnimationData animationData =  ref blobBuilder.ConstructRoot<AnimationData>();
            //    animationData.frameTimerMax = authoring.soldierIdle.frameTimerMax;
            //    animationData.frameMax = authoring.soldierIdle.meshArray.Length;

            //    BlobBuilderArray<BatchMeshID> blobBuilderArray = blobBuilder.Allocate<BatchMeshID>(ref animationData.batchMeshIdBlobArray, authoring.soldierIdle.meshArray.Length);


            //    for(int i=0;i<blobBuilderArray.Length;i++)
            //    {
            //        Mesh mesh = authoring.soldierIdle.meshArray[i];
            //        blobBuilderArray[i] = entitiesGraphicsSystem.RegisterMesh(mesh);
            //    }

            //    animationDataHolder.soldierIdle = blobBuilder.CreateBlobAssetReference<AnimationData>(Allocator.Persistent);

            //    blobBuilder.Dispose();

            //    AddBlobAsset(ref animationDataHolder.soldierIdle,out Unity.Entities.Hash128 objectHash);

            //    AddComponent(entity, animationDataHolder);
            //}


            BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
            ref BlobArray<AnimationData> animationDataBlobArray = ref blobBuilder.ConstructRoot<BlobArray<AnimationData>>();

            BlobBuilderArray<AnimationData> animationDataBlobBuilderArray = blobBuilder.Allocate<AnimationData>(ref animationDataBlobArray, System.Enum.GetValues(typeof(AnimationDataSO.AnimationType)).Length);


            int index = 0;

            foreach(AnimationDataSO.AnimationType animationType in System.Enum.GetValues(typeof(AnimationDataSO.AnimationType)))
            {

                AnimationDataSO animationDataSO = authoring.animationDataListSO.GetAnimationDataSO(animationType);

                BlobBuilderArray<BatchMeshID> blobBuilderArray = blobBuilder.Allocate<BatchMeshID>(ref animationDataBlobBuilderArray[index].batchMeshIdBlobArray, animationDataSO.meshArray.Length);

                animationDataBlobBuilderArray[index].frameTimerMax = animationDataSO.frameTimerMax;
                animationDataBlobBuilderArray[index].frameMax = animationDataSO.meshArray.Length;

                for(int i=0;i<blobBuilderArray.Length;i++)
                {
                    Mesh mesh = animationDataSO.meshArray[i];
                    blobBuilderArray[i] = entitiesGraphicsSystem.RegisterMesh(mesh);
                }

                index++;
            }

            //{
            //    BlobBuilderArray<BatchMeshID> blobBuilderArray = blobBuilder.Allocate<BatchMeshID>(ref animationDataBlobBuilderArray[1].batchMeshIdBlobArray, authoring.soldierWalk.meshArray.Length);

            //    animationDataBlobBuilderArray[1].frameTimerMax = authoring.soldierWalk.frameTimerMax;
            //    animationDataBlobBuilderArray[1].frameMax = authoring.soldierWalk.meshArray.Length;

            //    for(int i=0;i<blobBuilderArray.Length;i++)
            //    {
            //        Mesh mesh = authoring.soldierWalk.meshArray[i];
            //        blobBuilderArray[i] = entitiesGraphicsSystem.RegisterMesh(mesh);
            //    }
            //}

            animationDataHolder.animationDataBlobArrayBlobAssetReference = blobBuilder.CreateBlobAssetReference<BlobArray<AnimationData>>(Allocator.Persistent);

            blobBuilder.Dispose();


            AddBlobAsset(ref animationDataHolder.animationDataBlobArrayBlobAssetReference,out Unity.Entities.Hash128 hash128);

            AddComponent(entity, animationDataHolder);
        }
    }
}


public struct AnimationDataHolder : IComponentData
{
    public BlobAssetReference<BlobArray<AnimationData>> animationDataBlobArrayBlobAssetReference;
}


public struct AnimationData
{
    public float frameTimerMax;
    public int frameMax;
    public BlobArray<BatchMeshID> batchMeshIdBlobArray;
}
