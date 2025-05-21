using Unity.Entities;
using UnityEngine;
using Unity.Collections;

public class UnitSelectionManager : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover,Selected>().Build(entityManager);


            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

            NativeArray<UnitMover> unitMoverArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);

            for(int i=0;i<unitMoverArray.Length;i++)
            {
                UnitMover unitMover = unitMoverArray[i];    

                unitMover.targetPosition = mouseWorldPosition;

                //entityManager.SetComponentData(entityArray[i], unitMover);

                unitMoverArray[i] = unitMover;
            }

            entityQuery.CopyFromComponentDataArray(unitMoverArray);

        }
    }
}
