using UnityEngine;
using System.Collections;

public class WeaponPos : Photon.MonoBehaviour {

    public float mouseSensitivity = 0.5f;
    public Animator anim;
    [SerializeField] Vector3 localPosition;
    [SerializeField] Vector3 localEulerAngles;

    private float maxRotation;
    private float minRotation;
    private float rotationY = 0;

    // Called when game start
    void Start() {
        transform.parent = anim.GetBoneTransform(HumanBodyBones.RightHand).transform;
        transform.localPosition = localPosition;
        transform.localEulerAngles = localEulerAngles;
        maxRotation = localEulerAngles.x + 6f;
        minRotation = localEulerAngles.x - 14f;
    }

    // This function is called every fixed framerate frame
    void FixedUpdate() {
        if (photonView.isMine) {
            rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            rotationY = Mathf.Clamp(rotationY, minRotation - localEulerAngles.x, maxRotation - localEulerAngles.x);
            transform.localEulerAngles = new Vector3(rotationY + localEulerAngles.x, localEulerAngles.y, localEulerAngles.z);
        }
    }

    // Synchronize data on the network
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(transform.localEulerAngles.x);
        } else {
            transform.localEulerAngles = new Vector3 ((float)stream.ReceiveNext(), localEulerAngles.y, localEulerAngles.z);
        }
    }

}
