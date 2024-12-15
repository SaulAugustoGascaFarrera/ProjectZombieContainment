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

        foreach((RefRW<ActiveAnimation> activeAnimation,RefRW<MaterialMeshInfo> materialMeshInfo) in SystemAPI.Query<RefRW<ActiveAnimation>, RefRW<MaterialMeshInfo>>())
        {
            //if(!activeAnimation.ValueRO.animationDataBlobAssetReference.IsCreated)
            //{
            //    activeAnimation.ValueRW.animationDataBlobAssetReference = animationDataHolder.soldierIdle;
            //}


            if(Input.GetKey(KeyCode.T))
            {
                activeAnimation.ValueRW.activeAnimationType = AnimationDataSO.AnimationType.SoldierIdle;
            }

            if (Input.GetKey(KeyCode.I))
            {
                activeAnimation.ValueRW.activeAnimationType = AnimationDataSO.AnimationType.SoldierWalk;
            }


            ref AnimationData animationData = ref animationDataHolder.animationDataBlobArrayBlobAssetReference.Value[(int)activeAnimation.ValueRW.activeAnimationType];

            activeAnimation.ValueRW.frameTimer += SystemAPI.Time.DeltaTime;

            if(activeAnimation.ValueRO.frameTimer > animationData.frameTimerMax)
            {
                activeAnimation.ValueRW.frameTimer -= animationData.frameTimerMax;

                activeAnimation.ValueRW.frame = (activeAnimation.ValueRO.frame + 1) % animationData.frameMax;

                //switch(activeAnimation.ValueRO.frame)
                //{
                //    default:
                //    case 0:
                //        materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.frame0;
                //        break;
                //     case 1:
                //        materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.frame1;
                //        break;
                //    case 2:
                //        materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.frame2;
                //        break;

                //}

                materialMeshInfo.ValueRW.MeshID = animationData.batchMeshIDBlobArray[activeAnimation.ValueRO.frame];
               

            }


            
        }
    }
  
}
