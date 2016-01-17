using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerHealth : Photon.MonoBehaviour {

	public delegate void Respawn(float time);
	public event Respawn RespawnMe;
	public delegate void SendMessage(string Message);
	public event SendMessage SendNetworkMessage;

	public int startingHealth = 100;
	public int currentHealth;
	public float sinkSpeed = 0.12f;
	public Slider healthSlider;
	public Image damageImage;
	public AudioClip deathClip;
	public AudioClip hurtClip;
	public float flashSpeed = 5f;
	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
	public Animator anim;

	AudioSource playerAudio;
	FirstPersonController fps;
	GunShooting playerShooting;
	CapsuleCollider capsuleCollider;
	IKControl ikControl;
	PlayerScore score;
	bool isDead;
	bool isSinking;
	bool damaged;

	void Awake() {
		playerAudio = GetComponent<AudioSource>();
		playerShooting = GetComponentInChildren<GunShooting>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		fps = GetComponent<FirstPersonController>();
		ikControl = GetComponentInChildren<IKControl>();
		score = GetComponent<PlayerScore>();
		healthSlider = GameObject.FindGameObjectWithTag("Screen").GetComponentInChildren<Slider>();
		damageImage = GameObject.FindGameObjectWithTag("Screen").transform.FindChild("DamageImage").GetComponent<Image>();
		currentHealth = startingHealth;
		healthSlider.value = currentHealth;
	}

	void Update() {
		if (damaged) {
			damageImage.color = flashColour;
		} else {
			damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}
		damaged = false;

		if (isSinking) {
			transform.Translate(Vector3.down * sinkSpeed * Time.deltaTime);
		}
	}

	[PunRPC]
	public void TakeDamage(int amount, string enemyName) {
		if (isDead) return;

		currentHealth -= amount;

		if (photonView.isMine) {
			damaged = true;
			healthSlider.value = currentHealth;
		}

		anim.SetTrigger("IsHurt");

		playerAudio.clip = hurtClip;
		playerAudio.Play();

		if (currentHealth <= 0) {
			Death(enemyName);
		}
	}

	[PunRPC]
	void Death(string enemyName) {
		isDead = true;
		capsuleCollider.isTrigger = true;

		playerShooting.DisableEffects();

		anim.SetTrigger("IsDead");

		playerAudio.clip = deathClip;
		playerAudio.Play();

		fps.enabled = false;
		playerShooting.enabled = false;
		ikControl.enabled = false;

		gameObject.transform.FindChild("NameTag").gameObject.SetActive(false);

		if (photonView.isMine) {

			if (SendNetworkMessage != null) {
				SendNetworkMessage(PhotonNetwork.player.name + " was killed by " + enemyName + "!");
			}

			if (RespawnMe != null) {
				RespawnMe(8.0f);
			}
			StartCoroutine("DestoryPlayer", 7.9f);
		}

		StartCoroutine("StartSinking", 2.5f);
	}

	[PunRPC]
	IEnumerator DestoryPlayer(float time) {
		yield return new WaitForSeconds(time);
		PhotonNetwork.Destroy(gameObject);
	}

	[PunRPC]
	IEnumerator StartSinking(float time) {
		yield return new WaitForSeconds(time);
		GetComponent<Rigidbody>().isKinematic = true;
		isSinking = true;	
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(currentHealth);
		} else {
			currentHealth = (int)stream.ReceiveNext();
		}
	}
}
