using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class GunShooting : Photon.MonoBehaviour {

	public int damagePerShot = 20;
	public float timeBetweenBullets = 0.2f;
	public float range = 100.0f;
	public Animator anim;

	float timer;
	Ray shootRay;
	RaycastHit shootHit;
	int shootableMask;
	ParticleSystem gunParticles;
	LineRenderer gunLine;
	AudioSource gunAudio;
	float effectsDisplayTime = 0.2f;
	Vector3 target;

	void Awake() {
		shootableMask = LayerMask.GetMask("Shootable");
		gunParticles = GetComponent<ParticleSystem>();
		gunLine = GetComponent<LineRenderer>();
		gunAudio = GetComponent<AudioSource>();
	}

	void Update() {
		if (photonView.isMine) {
			timer += Time.deltaTime;

			bool shooting = CrossPlatformInputManager.GetButton("Fire1");

			if (shooting && timer >= timeBetweenBullets && Time.timeScale != 0) {
				GetComponent<PhotonView>().RPC("Shoot", PhotonTargets.All);
			} 

			anim.SetBool("Firing", shooting);

			if (timer >= timeBetweenBullets * effectsDisplayTime) {
				GetComponent<PhotonView>().RPC("DisableEffects", PhotonTargets.All);
			}
		}
	}

	[PunRPC]
	public void DisableEffects() {
		gunLine.enabled = false;
	}

	[PunRPC]
	void Shoot() {
		timer = 0.0f;

		gunAudio.Play();

		gunParticles.Stop();
		gunParticles.Play();

		gunLine.enabled = true;
		gunLine.SetPosition(0, transform.position);

		if (photonView.isMine) {
			shootRay = Camera.main.ScreenPointToRay(new Vector3((Screen.width * 0.5f), (Screen.height * 0.5f), 0f));
			if (Physics.Raycast(shootRay, out shootHit, range, shootableMask)) {
				target = shootHit.point;
				shootHit.collider.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, damagePerShot, shootHit.point);
			} else {
				target = shootRay.origin + shootRay.direction * range;
			}
			gunLine.SetPosition(1, target);
		} else {
			if (Physics.Raycast(transform.position, transform.forward, out shootHit, range, shootableMask)) {
				target = shootHit.point;
			} else {
				target = transform.position + transform.forward * range;
			}
			gunLine.SetPosition(1, target);
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(anim.GetBool("Firing"));
		} else {
			anim.SetBool("Firing", (bool)stream.ReceiveNext());
		}
	}
}

