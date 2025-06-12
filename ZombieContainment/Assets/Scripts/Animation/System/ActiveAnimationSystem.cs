using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

partial struct ActiveAnimationSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AnimationDataHolder>();
    }


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        AnimationDataHolder animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();    

        foreach((RefRW<ActiveAnimation> activeAnimation,RefRW<MaterialMeshInfo> materialMeshInfo) in SystemAPI.Query<RefRW<ActiveAnimation>,RefRW<MaterialMeshInfo>>())
        {

            //if(!activeAnimation.ValueRO.animationDataBlobAssetReference.IsCreated)
            //{
            //    activeAnimation.ValueRW.animationDataBlobAssetReference = animationDataHolder.soldierIdle;
            //    continue;
            //}


            if(Input.GetKeyDown(KeyCode.T))
            {
                activeAnimation.ValueRW.activeAnimationType = AnimationDataSO.AnimationType.SoldierIdle;
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                activeAnimation.ValueRW.activeAnimationType = AnimationDataSO.AnimationType.SoldierWalk;
            }

            ref AnimationData animationData = ref animationDataHolder.animationDataBlobArrayBlobAssetReference.Value[(int)activeAnimation.ValueRO.activeAnimationType];

            activeAnimation.ValueRW.frameTimer += SystemAPI.Time.DeltaTime;

            //if(activeAnimation.ValueRW.frameTimer > activeAnimation.ValueRO.animationDataBlobAssetReference.Value.frameTimerMax)
            //{
            //    activeAnimation.ValueRW.frameTimer -= activeAnimation.ValueRO.animationDataBlobAssetReference.Value.frameTimerMax;

            //    activeAnimation.ValueRW.frame = (activeAnimation.ValueRO.frame + 1) % activeAnimation.ValueRO.animationDataBlobAssetReference.Value.frameMax;


            //    //switch(activeAnimation.ValueRO.frame)
            //    //{
            //    //    case 0:
            //    //        materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.frame0;
            //    //        break;
            //    //    case 1:
            //    //        materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.frame1;
            //    //        break;
            //    //}


            //    materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.animationDataBlobAssetReference.Value.batchMeshIdBlobArray[activeAnimation.ValueRO.frame];

               
            //}

            if(activeAnimation.ValueRO.frameTimer > animationData.frameTimerMax)
            {
                activeAnimation.ValueRW.frameTimer -= animationData.frameTimerMax;

                activeAnimation.ValueRW.frame = (activeAnimation.ValueRO.frame + 1) % animationData.frameMax;

                materialMeshInfo.ValueRW.MeshID = animationData.batchMeshIdBlobArray[activeAnimation.ValueRO.frame];
            }
        }
    }

   
}
