using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class GunShooting : Photon.MonoBehaviour {

    public int damagePerShot = 20;
    public float timeBetweenBullets = 0.2f;
    public float range = 100.0f;
    public Animator anim;

    private float timer;
    private Ray shootRay;
    private RaycastHit shootHit;
    private int shootableMask;
    private ParticleSystem gunParticles;
    private LineRenderer gunLine;
    private AudioSource gunAudio;

    // Called when script awake in editor
    void Awake() {
        shootableMask = LayerMask.GetMask("Shootable");
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (photonView.isMine) {
            timer += Time.deltaTime;

            bool shooting = CrossPlatformInputManager.GetButton("Fire1");

            // RPC call every client "Shoot" function
            if (shooting && timer >= timeBetweenBullets && Time.timeScale != 0) {
                GetComponent<PhotonView>().RPC("Shoot", PhotonTargets.All);
            }

            anim.SetBool("Firing", shooting);
        }
    }

    [PunRPC]
    public void DisableEffects() {
        gunLine.enabled = false;
    }

    // RPC function for shooting
    [PunRPC]
    void Shoot() {
        timer = 0.0f;

        gunAudio.Play();

        gunParticles.Stop();
        gunParticles.Play();

        // Only call when is the client itself
        if (photonView.isMine) {
            shootRay = Camera.main.ScreenPointToRay(new Vector3((Screen.width * 0.5f), (Screen.height * 0.5f), 0f));
            if (Physics.Raycast(shootRay, out shootHit, range, shootableMask)) {
                switch (shootHit.transform.gameObject.tag) {
                case "Player":
                    shootHit.collider.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, damagePerShot, PhotonNetwork.player.NickName);
                    PhotonNetwork.Instantiate("impactFlesh", shootHit.point, Quaternion.Euler(shootHit.normal.x - 90, shootHit.normal.y, shootHit.normal.z), 0);
                    break;
                case "Metal":
                    PhotonNetwork.Instantiate("impactMetal", shootHit.point, Quaternion.Euler(shootHit.normal.x - 90, shootHit.normal.y, shootHit.normal.z), 0);
                    break;
                case "Glass":
                    PhotonNetwork.Instantiate("impactGlass", shootHit.point, Quaternion.Euler(shootHit.normal.x - 90, shootHit.normal.y, shootHit.normal.z), 0);
                    break;
                case "Wood":
                    PhotonNetwork.Instantiate("impactWood", shootHit.point, Quaternion.Euler(shootHit.normal.x - 90, shootHit.normal.y, shootHit.normal.z), 0);
                    break;
                case "Brick":
                    PhotonNetwork.Instantiate("impactBrick", shootHit.point, Quaternion.Euler(shootHit.normal.x - 90, shootHit.normal.y, shootHit.normal.z), 0);
                    break;
                case "Concrete":
                    PhotonNetwork.Instantiate("impactConcrete", shootHit.point, Quaternion.Euler(shootHit.normal.x - 90, shootHit.normal.y, shootHit.normal.z), 0);
                    break;
                case "Dirt":
                    PhotonNetwork.Instantiate("impactDirt", shootHit.point, Quaternion.Euler(shootHit.normal.x - 90, shootHit.normal.y, shootHit.normal.z), 0);
                    break;
                case "Water":
                    PhotonNetwork.Instantiate("impactWater", shootHit.point, Quaternion.Euler(shootHit.normal.x - 90, shootHit.normal.y, shootHit.normal.z), 0);
                    break;
                default:
                    break;
                }
            }
        }
    }

    // Synchronize data on the network
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(anim.GetBool("Firing"));
        } else {
            anim.SetBool("Firing", (bool)stream.ReceiveNext());
        }
    }

}

