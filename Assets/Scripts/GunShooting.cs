using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class GunShooting : MonoBehaviour {

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
	Light gunLight;
	float effectsDisplayTime = 0.2f;

	void Awake() {
		shootableMask = LayerMask.GetMask("Shootable");
		gunParticles = GetComponent<ParticleSystem>();
		gunLine = GetComponent<LineRenderer>();
		gunAudio = GetComponent<AudioSource>();
		gunLight = GetComponent<Light>();
	}

	void Update() {
		timer += Time.deltaTime;

		bool shooting = CrossPlatformInputManager.GetButton("Fire1");

		if (shooting && timer >= timeBetweenBullets && Time.timeScale != 0) {
			Shoot();
		} 
			
		anim.SetBool("Firing", shooting);

		if (timer >= timeBetweenBullets * effectsDisplayTime) {
			DisableEffects();
		}
	}

	public void DisableEffects() {
		gunLine.enabled = false;
		gunLight.enabled = false;
	}

	void Shoot() {
		timer = 0.0f;

		gunAudio.Play();

		gunLight.enabled = true;
		gunParticles.Stop();
		gunParticles.Play();

		gunLine.enabled = true;
		gunLine.SetPosition(0, transform.position);

		shootRay = Camera.main.ScreenPointToRay(new Vector3((Screen.width * 0.5f), (Screen.height * 0.5f), 0f));

		if (Physics.Raycast(shootRay, out shootHit, range, shootableMask)) {
			PlayerHealth playerHealth = shootHit.collider.GetComponent<PlayerHealth>();
			if (playerHealth != null) {
				playerHealth.TakeDamage(damagePerShot, shootHit.point);
			}
			gunLine.SetPosition(1, shootHit.point);
		} else {
			gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
		}
	}
}

