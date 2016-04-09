using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : Photon.MonoBehaviour {

    [SerializeField] Text connectionText;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Camera sceneCamera;
    [SerializeField] GameObject[] playerModel;

    private GameObject player;
    private Queue<string> messages;
    private const int messageCount = 10;

    // Called when game start
    void Start() {
        messages = new Queue<string> (messageCount);
        PhotonNetwork.logLevel = PhotonLogLevel.Full;
        PhotonNetwork.ConnectUsingSettings("0.3");
        StartCoroutine("UpdateConnectionState");
    }

    // The coroutine function to update the connection state message
    IEnumerator UpdateConnectionState() {
        while (true) {
            connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();
            yield return null;
        }
    }

    // Callback function on joined lobby
	void OnJoinedLobby() {
		PhotonNetwork.player.name = "VR Player";
		RoomOptions roomOptions = new RoomOptions() {isVisible = true, maxPlayers = 12};
		PhotonNetwork.JoinOrCreateRoom("DefaultRoom", roomOptions, TypedLobby.Default);
    }

    // Callback function on joined room
    void OnJoinedRoom() {
        StopCoroutine ("UpdateConnectionState");
        connectionText.text = "";
		StartSpawnProcess(0.0f);
    }

    // Start spawn player
    void StartSpawnProcess(float spawnTime) {
        StartCoroutine(SpawnPlayer(spawnTime));
    }

    // The coroutine function for spawn player
    IEnumerator SpawnPlayer(float spawnTime) {
        yield return new WaitForSeconds(spawnTime);

        int playerIndex = Random.Range(0, playerModel.Length);
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        player = PhotonNetwork.Instantiate(playerModel[playerIndex].name, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation, 0);

		sceneCamera.enabled = false;

        player.GetComponent<PlayerHealth>().RespawnMe += StartSpawnProcess;
        player.GetComponent<PlayerHealth>().SendNetworkMessage += AddMessage;


        if (spawnTime == 0.0f)
            AddMessage("Player " + PhotonNetwork.player.name + " Joined Game.");
        else
            AddMessage("Player " + PhotonNetwork.player.name + " Respawned.");
    }

    // Add message to message panel
    void AddMessage(string message) {
        GetComponent<PhotonView>().RPC("AddMessage_RPC", PhotonTargets.All, message);
    }

    // The RPC function to call add message for each client
    [PunRPC]
    void AddMessage_RPC(string message) {
        messages.Enqueue(message);
        if (messages.Count > messageCount)
            messages.Dequeue();
    }

    // Callback function when player disconnected
    void OnPhotonPlayerDisconnected(PhotonPlayer other) {
        if (photonView.isMine)
            AddMessage("Player " + other.name + " Left Game.");
    }

    // Synchronize data on the network
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
        } else {
        }
    }

}
