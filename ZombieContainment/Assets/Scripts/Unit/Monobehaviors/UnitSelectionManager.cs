using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using System;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; private set; }

    public event EventHandler OnSelectionAreaStart;
    public event EventHandler OnSelectionAreaEnd;

    private Vector2 selectionStartMousePositon;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectionStartMousePositon = Input.mousePosition;

            OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
        }
         
        if(Input.GetMouseButtonUp(0))
        {
            Vector2 selectionEndMousePosition = Input.mousePosition;

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);

            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);


            NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);

            for (int i = 0; i < entityArray.Length; i++)
            {
                entityManager.SetComponentEnabled<Selected>(entityArray[i], false);

                Selected selected = selectedArray[i];

                selected.onSelected = false;
                selected.onDeselected = true;

                entityManager.SetComponentData(entityArray[i], selected);
            }

            


            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform,Unit>().WithPresent<Selected>().Build(entityManager);


            Rect selectionAreaRect = GetSelectionAreaRect();
            float selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
            float multipleSlectionSizeMin = 40.0f;
            bool isMultipleSelection = selectionAreaSize > multipleSlectionSizeMin;

          
            if(isMultipleSelection)
            {
                entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

                for (int i = 0; i < localTransformArray.Length; i++)
                {
                    LocalTransform unitLocalTransform = localTransformArray[i];
                    Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);

                    if (selectionAreaRect.Contains(unitScreenPosition))
                    {
                        //valid unit is inside selection area
                        entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
                        Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);
                        selected.onSelected = true;
                        selected.onDeselected = false;
                        entityManager.SetComponentData(entityArray[i], selected);
                    }
                }
            }
            else
            {
                //Single uinit select
                entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));

                PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();

                CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

                UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                

                RaycastInput raycastInput = new RaycastInput
                {
                    Start = cameraRay.GetPoint(0f),
                    End = cameraRay.GetPoint(9999f),
                    Filter = new CollisionFilter
                    {
                        BelongsTo = ~0u,
                        CollidesWith = 1u << GameAssets.UNIT_LAYERS,
                        GroupIndex = 0
                    }
                };

                if(collisionWorld.CastRay(raycastInput,out Unity.Physics.RaycastHit raycastHit))
                {
                    if(entityManager.HasComponent<Unit>(raycastHit.Entity) && entityManager.HasComponent<Selected>(raycastHit.Entity))
                    {
                        //Hit an Unit
                        entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);
                        Selected selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);
                        selected.onSelected = true;
                        selected.onDeselected = false;
                        entityManager.SetComponentData(raycastHit.Entity, selected);
                    }
                }
            }
           


            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
        }

        if(Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;


            EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);


            RaycastInput raycastInput = new RaycastInput
            {
                Start = cameraRay.GetPoint(0f),
                End = cameraRay.GetPoint(9999f),
                Filter = new CollisionFilter
                {
                     BelongsTo = ~0u,
                     CollidesWith = 1u << GameAssets.UNIT_LAYERS | 1u << GameAssets.BUILDING_LAYERS,
                     GroupIndex = 0
                }
            };

            bool isAttackingSingleTarget = false;

            if(collisionWorld.CastRay(raycastInput,out Unity.Physics.RaycastHit raycastHit))
            {
                if(entityManager.HasComponent<Faction>(raycastHit.Entity))
                {
                    //Hit something with faction component
                    Faction faction = entityManager.GetComponentData<Faction>(raycastHit.Entity);

                    if(faction.factionType == FactionType.Zombie)
                    {
                        isAttackingSingleTarget = true;

                        //right clicking on a zombie


                        entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().WithPresent<TargetOverride>().Build(entityManager);


                        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

                        NativeArray<TargetOverride> targetOverrideArray = entityQuery.ToComponentDataArray<TargetOverride>(Allocator.Temp);


                        for (int i = 0; i < targetOverrideArray.Length; i++)
                        {
                            TargetOverride targetOverride = targetOverrideArray[i];


                            targetOverride.targetEntity = raycastHit.Entity;

                            targetOverrideArray[i] = targetOverride;

                            entityManager.SetComponentEnabled<MoveOverride>(entityArray[i], false);
                        }

                        entityQuery.CopyFromComponentDataArray(targetOverrideArray);


                        //entityManager.SetComponentData(raycastHit.Entity,new TargetOverride
                        //{
                        //    targetEntity = raycastHit.Entity
                        //});
                    }
                }
            }


            if(!isAttackingSingleTarget)
            {
                entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().WithPresent<MoveOverride,TargetOverride>().Build(entityManager);


                NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

                NativeArray<MoveOverride> moveOverrideArray = entityQuery.ToComponentDataArray<MoveOverride>(Allocator.Temp);


                NativeArray<TargetOverride> targetOverrideArray = entityQuery.ToComponentDataArray<TargetOverride>(Allocator.Temp);

                NativeArray<float3> movePositionArray = GenerateMovePositionArray(mouseWorldPosition, entityArray.Length);

                for (int i = 0; i < moveOverrideArray.Length; i++)
                {

                    //mob=ve override
                    MoveOverride moveOverride = moveOverrideArray[i];

                    //unitMover.targetPosition = mouseWorldPosition;

                    //entityManager.SetComponentData(entityArray[i], unitMover);

                    moveOverride.targetPosition = movePositionArray[i];

                    moveOverrideArray[i] = moveOverride;

                    entityManager.SetComponentEnabled<MoveOverride>(entityArray[i], true);


                    //target override

                    TargetOverride targetOverride = targetOverrideArray[i];

                    targetOverride.targetEntity = Entity.Null;

                    targetOverrideArray[i] = targetOverride;
                }

                entityQuery.CopyFromComponentDataArray(moveOverrideArray);

                entityQuery.CopyFromComponentDataArray(targetOverrideArray);
            }

            

        }
    }


    public Rect GetSelectionAreaRect()
    {
        Vector2 selectionEndMousePosition = Input.mousePosition;

        Vector2 lowerLeftCorner = new Vector2(
             Mathf.Min(selectionStartMousePositon.x, selectionEndMousePosition.x),
             Mathf.Min(selectionStartMousePositon.y, selectionEndMousePosition.y)
         );
        Vector2 upperRightCorner = new Vector2(
            Mathf.Max(selectionStartMousePositon.x, selectionEndMousePosition.x),
            Mathf.Max(selectionStartMousePositon.y, selectionEndMousePosition.y)
        );

        return new Rect(lowerLeftCorner.x,lowerLeftCorner.y,upperRightCorner.x - lowerLeftCorner.x,upperRightCorner.y - lowerLeftCorner.y);
    }


    private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount)
    {
        NativeArray<float3> positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);
        if (positionCount == 0)
        {
            return positionArray;
        }
        positionArray[0] = targetPosition;

        if (positionCount == 1)
        {
            return positionArray;
        }

        float ringSize = 2.9f;
        int ring = 0;
        int positionIndex = 1;

        while (positionIndex < positionCount)
        {
            int ringPositionCount = 3 + ring * 2;

            for (int i = 0; i < ringPositionCount; i++)
            {
                float angle = i * (math.PI2 / ringPositionCount);
                float3 ringVector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (ring + 1), 0, 0));
                float3 ringPosition = targetPosition + ringVector;

                positionArray[positionIndex] = ringPosition;
                positionIndex++;

                if (positionIndex >= positionCount)
                {
                    break;
                }
            }
            ring++;
        }

        return positionArray;
    }
}
