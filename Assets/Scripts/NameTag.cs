using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NameTag : Photon.MonoBehaviour {

    public Transform target = null;

    // Called when game start
    void Start() {
        if (photonView.isMine)
            GetComponent<PhotonView>().RPC("SetName", PhotonTargets.All, PhotonNetwork.playerName);
    }

    // The RPC function to set the player name tag
    [PunRPC]
    void SetName(string name) {
        GetComponentInChildren<Text>().text = name;
    }

    // Update is called once per frame
    void Update() {
        if (target != null) {
            Vector3 v3 = transform.position + (transform.position - target.position);
            transform.LookAt(v3, Vector3.up);
        }
    }

    // Synchronize data on the network
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(PhotonNetwork.playerName);
        } else {
            GetComponentInChildren<Text>().text = (string)stream.ReceiveNext();
        }
    }
}
