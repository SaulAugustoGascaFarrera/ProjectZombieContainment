using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthBarSystem : ISystem
{
    

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Vector3 cameraForward = Vector3.zero;

        if(Camera.main != null)
        {
            cameraForward = Camera.main.transform.forward;
        }


        foreach((RefRW<LocalTransform> localTransform,RefRO<HealthBar> healthBar) in SystemAPI.Query<RefRW<LocalTransform>,RefRO<HealthBar>>())
        {

            LocalTransform parentLocalTransform = SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.healthEntity);
            

            if(localTransform.ValueRO.Scale == 1.0f)
            {
                //health bar is visible
                localTransform.ValueRW.Rotation = parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
            }


            Health healthEntity = SystemAPI.GetComponent<Health>(healthBar.ValueRO.healthEntity);

            if(!healthEntity.onHealthChanged)
            {
                continue;
            }

            float healthNormalized = (float)healthEntity.healthAmount / healthEntity.healthAmountMax;

            if(healthNormalized == 1.0f)
            {
                localTransform.ValueRW.Scale = 0.0f;
            }
            else
            {
                localTransform.ValueRW.Scale = 1.0f;
            }

            //RefRW<LocalTransform> barVisualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(healthBar.ValueRO.barVisualEntity);
            //barVisualLocalTransform.ValueRW.Scale = healthNormalized / 2;

            RefRW<PostTransformMatrix> barVisualPostTransformMatrix = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisualEntity);
            barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1, 1);
        }
    }

    
}
