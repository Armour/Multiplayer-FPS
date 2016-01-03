using UnityEngine;
using System.Collections;

[System.Serializable]
public class PhotonTransformViewRotationModel 
{
    public enum InterpolateOptions
    {
        Disabled,
        RotateTowards,
        Lerp,
    }

    public bool SynchronizeEnabled;

    public InterpolateOptions InterpolateOption = InterpolateOptions.RotateTowards;
    public float InterpolateRotateTowardsSpeed = 180;
    public float InterpolateLerpSpeed = 5;
}
