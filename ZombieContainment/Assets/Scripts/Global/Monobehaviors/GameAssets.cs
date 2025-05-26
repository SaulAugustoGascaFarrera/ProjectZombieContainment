using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public const int UNIT_LAYERS = 6;
        
    public static GameAssets Instance {  get; private set; } 
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
    }
}
