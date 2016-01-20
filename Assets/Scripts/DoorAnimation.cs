using UnityEngine;
using System.Collections;

public class DoorAnimation : MonoBehaviour {

	Animator anim;
	float stx;
	float stz;

	void Awake() {
		stx = transform.position.x;
		stz = transform.position.z;
	}

	void Start() {
		anim = GetComponent<Animator>();
	}

	void Update() {
		transform.position = new Vector3(stx, Mathf.Clamp(transform.position.y, 2.74f, 5.9f), stz);
	}

	void OnTriggerStay(Collider other) {
		if (other.gameObject.tag == "Player")
			anim.SetBool("Trigger", true);
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Player")
			anim.SetBool("Trigger", false);
	}
}
