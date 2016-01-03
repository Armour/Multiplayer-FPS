using UnityEngine;
using System.Collections;

[System.Serializable]
public class PhotonTransformViewScaleModel 
{
    public enum InterpolateOptions
    {
        Disabled,
        MoveTowards,
        Lerp,
    }

    public bool SynchronizeEnabled;

    public InterpolateOptions InterpolateOption = InterpolateOptions.Disabled;
    public float InterpolateMoveTowardsSpeed = 1f;
    public float InterpolateLerpSpeed;
}
