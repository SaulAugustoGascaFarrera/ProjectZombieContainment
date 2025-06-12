using UnityEngine;


[CreateAssetMenu()]
public class AnimationDataSO : ScriptableObject
{

    public enum AnimationType
    {
        None,
        SoldierIdle,
        SoldierWalk
    }

    public AnimationType animationType;
    public Mesh[] meshArray;
    public float frameTimerMax;

}
