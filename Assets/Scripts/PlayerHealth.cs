using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerHealth : MonoBehaviour {

	public int startingHealth = 100;
	public int currentHealth;
	public float sinkSpeed = 0.08f;
	public Slider healthSlider;
	public Image damageImage;
	public AudioClip deathClip;
	public AudioClip hurtClip;
	public float flashSpeed = 5f;
	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

	Animator anim;
	AudioSource playerAudio;
	FirstPersonController fps;
	GunShooting playerShooting;
	ParticleSystem hitParticles;
	CapsuleCollider capsuleCollider;
	IKControl ikControl;
	bool isDead;
	bool isSinking;
	bool damaged;

	void Awake() {
		anim = GetComponentInChildren<Animator>();
		playerAudio = GetComponent<AudioSource>();
		hitParticles = GetComponentInChildren<ParticleSystem>();
		playerShooting = GetComponentInChildren<GunShooting>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		fps = GetComponent<FirstPersonController>();
		ikControl = GetComponentInChildren<IKControl>();
		currentHealth = startingHealth;
	}

	void Update() {
		if (damaged) {
			damageImage.color = flashColour;
		} else {
			damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}
		damaged = false;

		if(isSinking) {
			transform.Translate(Vector3.down * sinkSpeed * Time.deltaTime);
		}
	}

	public void TakeDamage(int amount, Vector3 hitPoint) {
		if (isDead) return;

		damaged = true;
		currentHealth -= amount;
		healthSlider.value = currentHealth;
		anim.SetTrigger("IsHurt");

		playerAudio.clip = hurtClip;
		playerAudio.Play();

		hitParticles.transform.position = hitPoint;
		hitParticles.Play();

		if (currentHealth <= 0) {
			Death();
		}
	}

	void Death() {
		isDead = true;
		capsuleCollider.isTrigger = true;

		playerShooting.DisableEffects();

		anim.SetTrigger("IsDead");

		playerAudio.clip = deathClip;
		playerAudio.Play();

		fps.enabled = false;
		playerShooting.enabled = false;
		ikControl.enabled = false;

		StartCoroutine("StartSinking");
	}

	IEnumerator StartSinking() {
		yield return new WaitForSeconds(2.5f);
		GetComponent<Rigidbody>().isKinematic = true;
		isSinking = true;
		Destroy(gameObject, 7f);
	}

	public void RestartLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
