using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class GunFirstPersonView : MonoBehaviour {

    public float timeBetweenBullets = 0.2f;
    public float range = 100.0f;
    public Animator anim;

    private float timer;
    private Ray shootRay;
    private RaycastHit shootHit;
    private ParticleSystem gunParticles;
    private LineRenderer gunLine;

    // Called when script awake in editor
    void Awake() {
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;

        bool shooting = CrossPlatformInputManager.GetButton("Fire1");

        if (shooting && timer >= timeBetweenBullets && Time.timeScale != 0) {
            Shoot();
        }

        anim.SetBool("Firing", shooting);
    }

    // Disable the shooting effects
    public void DisableEffects() {
        gunLine.enabled = false;
    }

    // Shoot!
    void Shoot() {
        timer = 0.0f;
        gunParticles.Stop();
        gunParticles.Play();
    }

}

