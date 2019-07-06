using UnityEngine;

public class CameraRotation : MonoBehaviour {

    [SerializeField]
    private Transform lookPoint;
    [SerializeField]
    private float rotateSpeed = 12f;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update() {
        transform.RotateAround(lookPoint.position, Vector3.up, rotateSpeed * Time.deltaTime);
        transform.LookAt(lookPoint, Vector3.up);
    }

}
