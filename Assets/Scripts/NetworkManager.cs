using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {
	
	[SerializeField] Text connectionText;
	[SerializeField] Transform[] spawnPoints;
	[SerializeField] Camera sceneCamera;
	[SerializeField] GameObject[] playerModel; 
	[SerializeField] GameObject serverWindow;
	[SerializeField] GameObject messageWindow;
	[SerializeField] GameObject sightImage;
	[SerializeField] InputField username;
	[SerializeField] InputField roomName;
	[SerializeField] InputField roomList;
	[SerializeField] InputField messagesLog;

	GameObject player;
	Queue<string> messages;
	const int messageCount = 10;
	PhotonView photonView;

	void Start() {
		photonView = GetComponent<PhotonView>();
		messages = new Queue<string> (messageCount);
		PhotonNetwork.logLevel = PhotonLogLevel.Full;
		PhotonNetwork.ConnectUsingSettings("0.1");
		StartCoroutine("UpdateConnectionState");
	}
		
	IEnumerator UpdateConnectionState() {
		while(true) {
			connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();
			yield return null;
		}
	}

	void OnJoinedLobby() {
		serverWindow.SetActive (true);
	}

	void OnReceivedRoomListUpdate() {
		roomList.text = "";
		RoomInfo[] rooms = PhotonNetwork.GetRoomList();
		foreach (RoomInfo room in rooms)
			roomList.text += room.name + "\n";
	}

	void OnJoinedRoom() {
		StopCoroutine ("UpdateConnectionState");
		connectionText.text = "";
		StartSpawnProcess(0.0f);
	}

	public void JoinRoom() {
		serverWindow.SetActive(false);
		PhotonNetwork.player.name = username.text;
		RoomOptions roomOptions = new RoomOptions() {isVisible = true, maxPlayers = 12};
		PhotonNetwork.JoinOrCreateRoom(roomName.text, roomOptions, TypedLobby.Default);
	}

	void StartSpawnProcess(float spawnTime) {
		sightImage.SetActive(false);
		sceneCamera.enabled = true;
		StartCoroutine(SpawnPlayer(spawnTime));
	}

	IEnumerator SpawnPlayer(float spawnTime) {
		yield return new WaitForSeconds(spawnTime);

		messageWindow.SetActive(true);
		sightImage.SetActive(true);
		int playerIndex = Random.Range(0, playerModel.Length);
		int spawnIndex = Random.Range(0, spawnPoints.Length);
		player = PhotonNetwork.Instantiate(playerModel[playerIndex].name, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation, 0);

		player.GetComponent<PlayerHealth>().RespawnMe += StartSpawnProcess;
		player.GetComponent<PlayerHealth>().SendNetworkMessage += AddMessage;

		sceneCamera.enabled = false;

		if (spawnTime == 0.0f)
			AddMessage("Player " + PhotonNetwork.player.name + " Joined Game.");
		else
			AddMessage("Player " + PhotonNetwork.player.name + " Respawned.");
	}

	void AddMessage(string message) {
		photonView.RPC("AddMessage_RPC", PhotonTargets.All, message);
	}

	[PunRPC]
	void AddMessage_RPC(string message) {
		messages.Enqueue(message);
		if (messages.Count > messageCount)
			messages.Dequeue();

		messagesLog.text = "";
		foreach (string m in messages)
			messagesLog.text += m + "\n";
	}

	void OnLeftRoom() {
		AddMessage("Player " + PhotonNetwork.player.name + " Left Game.");
	}
}
