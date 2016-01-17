using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NameTag : Photon.MonoBehaviour {

	public Transform target = null;

	void Start () {
		if (photonView.isMine)
			GetComponent<PhotonView>().RPC("SetName", PhotonTargets.All, PhotonNetwork.playerName);
	}

	[PunRPC]
	void SetName(string name) {
		GetComponentInChildren<Text>().text = name;
	}

	void Update() {
		if (target != null) {
			Vector3 v3 = transform.position + (transform.position - target.position);
			transform.LookAt(v3, Vector3.up);
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(PhotonNetwork.playerName);
		} else {
			GetComponent<Text>().text = (string)stream.ReceiveNext();
		}
	}
}
