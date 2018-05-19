using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : Photon.MonoBehaviour {

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

    private GameObject player;
    private Queue<string> messages;
    private const int messageCount = 10;

    // Called when game start
    void Start() {
        messages = new Queue<string> (messageCount);
        PhotonNetwork.logLevel = PhotonLogLevel.Full;
        PhotonNetwork.ConnectUsingSettings("0.2");
        StartCoroutine("UpdateConnectionState");
    }

    // The coroutine function to update the connection state message
    IEnumerator UpdateConnectionState() {
        while (true) {
            connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();
            yield return null;
        }
    }

    // Callback function on connected to master
    void OnConnectedToMaster() {
        PhotonNetwork.JoinLobby();
    }

    // Callback function on joined lobby
    void OnJoinedLobby() {
        serverWindow.SetActive(true);
    }

    // Callback function on reveived room list update
    void OnReceivedRoomListUpdate() {
        roomList.text = "";
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        foreach (RoomInfo room in rooms)
            roomList.text += room.Name + "\n";
    }

    // Callback function on joined room
    void OnJoinedRoom() {
        StopCoroutine ("UpdateConnectionState");
        connectionText.text = "";
        StartSpawnProcess(0.0f);
    }

    // The button click callback function for join room
    public void JoinRoom() {
        serverWindow.SetActive(false);
        PhotonNetwork.player.NickName = username.text;
        RoomOptions roomOptions = new RoomOptions() {IsVisible = true, MaxPlayers = 12};
        PhotonNetwork.JoinOrCreateRoom(roomName.text, roomOptions, TypedLobby.Default);
    }

    // Start spawn player
    void StartSpawnProcess(float spawnTime) {
        sightImage.SetActive(false);
        sceneCamera.enabled = true;
        StartCoroutine(SpawnPlayer(spawnTime));
    }

    // The coroutine function for spawn player
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
            AddMessage("Player " + PhotonNetwork.player.NickName + " Joined Game.");
        else
            AddMessage("Player " + PhotonNetwork.player.NickName + " Respawned.");
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

        messagesLog.text = "";
        foreach (string m in messages)
            messagesLog.text += m + "\n";
    }

    // Callback function when player disconnected
    void OnPhotonPlayerDisconnected(PhotonPlayer other) {
        if (photonView.isMine)
            AddMessage("Player " + other.NickName + " Left Game.");
    }

    // Synchronize data on the network
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
        } else {
        }
    }

}
