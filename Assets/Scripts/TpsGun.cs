using Photon.Pun;
using UnityEngine;

public class TpsGun : MonoBehaviourPunCallbacks, IPunObservable {

    [Tooltip("The scaling number for changing the local postion Y of TpsGun when aiming angle changes.")]
    [SerializeField]
    private float localPositionYScale = 0.007f;
    [SerializeField]
    private ParticleSystem gunParticles;
    [SerializeField]
    private AudioSource gunAudio;
    [SerializeField]
    private FpsGun fpsGun;
    [SerializeField]
    private Animator animator;

    private float timer;
    private Vector3 localPosition;
    private Quaternion localRotation;
    private float smoothing = 2.0f;
    private float defaultLocalPositionY;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start() {
        if (photonView.IsMine) {
            defaultLocalPositionY = transform.localPosition.y;
        } else {
            localPosition = transform.localPosition;
            localRotation = transform.localRotation;
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update() {
        if (photonView.IsMine) {
            transform.rotation = fpsGun.transform.rotation;
        }
    }

    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    void LateUpdate() {
        if (photonView.IsMine) {
            float deltaEulerAngle = 0f;
            if (transform.eulerAngles.x > 180) {
                deltaEulerAngle = 360 - transform.eulerAngles.x;
            } else {
                deltaEulerAngle = -transform.eulerAngles.x;
            }
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                defaultLocalPositionY + deltaEulerAngle * localPositionYScale,
                transform.localPosition.z
            );
        } else {
            transform.localPosition = Vector3.Lerp(transform.localPosition, localPosition, Time.deltaTime * smoothing);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, localRotation, Time.deltaTime * smoothing);
        }
    }

    /// <summary>
    /// Public function to call RPC shoot.
    /// </summary>
    public void RPCShoot() {
        if (photonView.IsMine) {
            photonView.RPC("Shoot", RpcTarget.All);
        }
    }

    /// <summary>
    /// RPC function to shoot once.
    /// </summary>
    [PunRPC]
    void Shoot() {
        gunAudio.Play();
        if (!photonView.IsMine) {
            if (gunParticles.isPlaying) {
                gunParticles.Stop();
            }
            gunParticles.Play();
        }
    }

    /// <summary>
    /// Used to customize synchronization of variables in a script watched by a photon network view.
    /// </summary>
    /// <param name="stream">The network bit stream.</param>
    /// <param name="info">The network message information.</param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(transform.localPosition);
            stream.SendNext(transform.localRotation);
        } else {
            localPosition = (Vector3)stream.ReceiveNext();
            localRotation = (Quaternion)stream.ReceiveNext();
        }
    }

}
