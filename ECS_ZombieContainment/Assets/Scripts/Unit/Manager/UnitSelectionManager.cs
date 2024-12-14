using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance;

    public event EventHandler OnSelectionAreaStart;
    public event EventHandler OnSelectionAreaEnd;

    private Vector2 selectionStartMousePosition;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There is UnitSelectionManager Instance");
        }

        Instance = this;
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            selectionStartMousePosition = Input.mousePosition;
            OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
        }

        if(Input.GetMouseButtonUp(0))
        {
            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);

            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);

            for (int i=0;i<entityArray.Length; i++)
            {
                Selected selected = selectedArray[i];

                entityManager.SetComponentEnabled<Selected>(entityArray[i], false);

                //selected.OnSelected = false;
                selected.OnDeselected = true;

       

                entityManager.SetComponentData<Selected>(entityArray[i],selected);
            }

           

            Rect selectionAreaRect = GetSelectionAreaRect();
            float selectionAreaSize = selectionAreaRect.width * selectionAreaRect.height;
            float multipleSelectionAreaMin = 40.0f;
            bool isMultipleSelection = selectionAreaSize > multipleSelectionAreaMin;

            if(isMultipleSelection)
            {
                entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, UnitMover>().WithDisabled<Selected>().Build(entityManager);

                entityArray = entityQuery.ToEntityArray(Allocator.Temp);

                NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                //selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);

                for (int i = 0; i < localTransformArray.Length; i++)
                {
                    LocalTransform localTransform = localTransformArray[i];

                    //Selected selected = selectedArray[i];

                    Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);

                    Vector2 unitPosition = Camera.main.WorldToScreenPoint(localTransform.Position);

                    if (selectionAreaRect.Contains(unitPosition))
                    {
                        entityManager.SetComponentEnabled<Selected>(entityArray[i], true);

                        selected.OnSelected = true;
                        //selected.OnDeselected = false;

                        entityManager.SetComponentData(entityArray[i], selected);
                    }


                }
            }
            else
            {
                entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));

                

                PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();

                CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

                UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastInput raycastInput = new RaycastInput
                {
                    Start = ray.GetPoint(0),
                    End = ray.GetPoint(9999.0f),
                    Filter = new CollisionFilter
                    {
                        BelongsTo = ~0u,
                        CollidesWith = 1u << GameAssets.UNIT_LAYER,
                        GroupIndex = 0,

                    }
                    
                };

                if(collisionWorld.CastRay(raycastInput,out Unity.Physics.RaycastHit raycastHit))
                {
                    if(entityManager.HasComponent<UnitMover>(raycastHit.Entity) && entityManager.HasComponent<Selected>(raycastHit.Entity))
                    {
                        Selected selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);

                        entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);

                        selected.OnSelected = true;
                        //selected.OnDeselected = false;
                        
                        entityManager.SetComponentData(raycastHit.Entity,selected);

                    }
                }

            }


           



        }

        if(Input.GetMouseButtonDown(1))
        {
            Vector3 MousePosition = MousePositionManager.Instance.GetMousePosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            //EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(Selected),typeof(MoveOverride));
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().WithPresent<MoveOverride>().Build(entityManager);

            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<MoveOverride> moveOverrideArray = entityQuery.ToComponentDataArray<MoveOverride>(Allocator.Temp);

            for (int i = 0; i < entityArray.Length; i++)
            {
                MoveOverride moveOverride = moveOverrideArray[i];

                moveOverride.targetPosition = MousePosition;

                moveOverrideArray[i] = moveOverride;

                //entityManager.SetComponentData(entityArray[i], moveOverride);

                entityManager.SetComponentEnabled<MoveOverride>(entityArray[i], true);

                //Debug.Log(MousePosition);
            }

            entityQuery.CopyFromComponentDataArray(moveOverrideArray);

        }
    }

    public Rect GetSelectionAreaRect()
    {
        Vector2 selectionEndMousePosition = Input.mousePosition;

        Vector2 lowerLeftCorner = new Vector2(Mathf.Min(selectionStartMousePosition.x, selectionEndMousePosition.x), Mathf.Min(selectionStartMousePosition.y,selectionEndMousePosition.y));

        Vector2 upperRightCorner = new Vector2(Mathf.Max(selectionStartMousePosition.x,selectionEndMousePosition.x),Mathf.Max(selectionStartMousePosition.y,selectionEndMousePosition.y));

        Rect selectionRect = new Rect(lowerLeftCorner.x,lowerLeftCorner.y,upperRightCorner.x - lowerLeftCorner.x,upperRightCorner.y - lowerLeftCorner.y);

        return selectionRect;
    }


    private NativeArray<float3> GenerateMovePositionArray2(float3 targetPosition,int positionCount)
    {
        NativeArray<float3> positionArray = new NativeArray<float3>(positionCount,Allocator.Temp);

        if (positionCount == 0)
        {
            return positionArray;
        }

        //if(positionCount == 1)
        //{
        //    return positionArray;
        //}

        for(int i=0;i<positionArray.Length;i++)
        {

            float angle = i * (2.0f * Mathf.PI / positionCount);

            //Debug.Log(angle);

            float3 position = new float3(math.cos(angle),0.0f,math.sin(angle)) * 5.0f + targetPosition;

            positionArray[i] = position;
        }

        return positionArray;
    }


}
