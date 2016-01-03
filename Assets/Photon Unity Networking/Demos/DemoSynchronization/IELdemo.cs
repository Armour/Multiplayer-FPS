using ExitGames.Client.Photon;
using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class IELdemo : MonoBehaviour
{
    public Transform[] cubes;

    #region CONNECTION HANDLING

    public void Awake()
    {
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.ConnectUsingSettings("0.9");
        }
    }

    // This is one of the callback/event methods called by PUN (read more in PhotonNetworkingMessage enumeration)
    public void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    // This is one of the callback/event methods called by PUN (read more in PhotonNetworkingMessage enumeration)
    public void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);
    }

    // This is one of the callback/event methods called by PUN (read more in PhotonNetworkingMessage enumeration)
    public void OnJoinedRoom()
    {
    }

    // This is one of the callback/event methods called by PUN (read more in PhotonNetworkingMessage enumeration)
    public void OnCreatedRoom()
    {
    }

    #endregion

    public void Update()
    {
        if (PhotonNetwork.isMasterClient)
        {
            //Only control the cubes if MC
            float movement = Input.GetAxis("Horizontal") * Time.deltaTime * 15;
            foreach (Transform tran in cubes)
            {
                tran.position += new Vector3(movement, 0, 0);
            }
        }
    }

    public void OnGUI()
    {
        GUILayout.Space(10);
        if (PhotonNetwork.isMasterClient)
        {
            GUILayout.Label("Move the cubes with the left and right keys. Run another client to check movement (smoothing) behaviour.");
            GUILayout.Label("Ping: " + PhotonNetwork.GetPing());
        }
        else if (PhotonNetwork.isNonMasterClientInRoom)
        {
            GUILayout.Label("Check how smooth the movement is");
            GUILayout.Label("Ping: " + PhotonNetwork.GetPing());
        }
        else
        {
            GUILayout.Label("Not connected..." + PhotonNetwork.connectionStateDetailed);
        }
    }
}
