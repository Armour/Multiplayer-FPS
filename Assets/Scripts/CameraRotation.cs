using UnityEngine;
using System.Collections;

public class CameraRotation : MonoBehaviour {

    public Transform point;

    private Vector3 up;

    // Use this for initialization
    void Start() {
        up = transform.up;
        transform.LookAt(point, up);
    }

    // Update is called once per frame
    void Update() {
        transform.RotateAround(point.position, Vector3.up, 12 * Time.deltaTime);
        transform.LookAt(point, up);
    }

}
