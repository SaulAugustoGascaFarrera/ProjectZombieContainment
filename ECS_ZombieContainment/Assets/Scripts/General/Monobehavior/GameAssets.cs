using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public const int UNIT_LAYER = 7;

    public static GameAssets Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
}
