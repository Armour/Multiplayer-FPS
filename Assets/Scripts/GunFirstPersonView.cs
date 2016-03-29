using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class GunFirstPersonView : MonoBehaviour {

	public float timeBetweenBullets = 0.2f;
	public float range = 100.0f;
	public Animator anim;

	float timer;
	Ray shootRay;
	RaycastHit shootHit;
	ParticleSystem gunParticles;
	LineRenderer gunLine;

	void Awake() {
		gunParticles = GetComponent<ParticleSystem>();
		gunLine = GetComponent<LineRenderer>();
	}

	void Update() {
		timer += Time.deltaTime;

		bool shooting = CrossPlatformInputManager.GetButton("Fire1");

		if (shooting && timer >= timeBetweenBullets && Time.timeScale != 0) {
			Shoot();
		} 
			
		anim.SetBool("Firing", shooting);
	}

	public void DisableEffects() {
		gunLine.enabled = false;
	}

	void Shoot() {
		timer = 0.0f;
		gunParticles.Stop();
		gunParticles.Play();
	}
}

