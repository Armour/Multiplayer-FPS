using UnityEngine;
using System.Collections;

public class NameTag : MonoBehaviour {

	public Transform target = null;

	void Start () {
		GetComponent<TextMesh>().text = PhotonNetwork.playerName;
	}

	void Update() {
		if (target != null) {
			Vector3 v3 = transform.position + (transform.position - target.position);
			transform.LookAt(v3, Vector3.up);
		}
	}
}
