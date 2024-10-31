using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class MousePositionManager : MonoBehaviour
{
    public static MousePositionManager Instance;
    //[SerializeField] private LayerMask 

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There more than one MousePositionManager Instance");
        }

        Instance = this;
    }


    public Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if(Physics.Raycast(ray,out RaycastHit raycastHit,float.MaxValue,1 << 6))
        {
            //Debug.Log(raycastHit.point);

            return raycastHit.point;
        }


        return Vector3.zero;
    }


}
