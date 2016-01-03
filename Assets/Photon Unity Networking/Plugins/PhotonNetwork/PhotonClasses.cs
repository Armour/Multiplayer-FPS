// ----------------------------------------------------------------------------
// <copyright file="PhotonClasses.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#pragma warning disable 1587
/// \file
/// <summary>Wraps up smaller classes that don't need their own file. </summary>
///
///
/// \defgroup publicApi Public API
/// \brief Groups the most important classes that you need to understand early on.
///
/// \defgroup optionalGui Optional Gui Elements
/// \brief Useful GUI elements for PUN.
#pragma warning restore 1587

#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
#define UNITY_MIN_5_3
#endif

using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;


/// <summary>Defines the OnPhotonSerializeView method to make it easy to implement correctly for observable scripts.</summary>
/// \ingroup publicApi
public interface IPunObservable
{
    /// <summary>
    /// Called by PUN several times per second, so that your script can write and read synchronization data for the PhotonView.
    /// </summary>
    /// <remarks>
    /// This method will be called in scripts that are assigned as Observed component of a PhotonView.<br/>
    /// PhotonNetwork.sendRateOnSerialize affects how often this method is called.<br/>
    /// PhotonNetwork.sendRate affects how often packages are sent by this client.<br/>
    ///
    /// Implementing this method, you can customize which data a PhotonView regularly synchronizes.
    /// Your code defines what is being sent (content) and how your data is used by receiving clients.
    ///
    /// Unlike other callbacks, <i>OnPhotonSerializeView only gets called when it is assigned
    /// to a PhotonView</i> as PhotonView.observed script.
    ///
    /// To make use of this method, the PhotonStream is essential. It will be in "writing" mode" on the
    /// client that controls a PhotonView (PhotonStream.isWriting == true) and in "reading mode" on the
    /// remote clients that just receive that the controlling client sends.
    ///
    /// If you skip writing any value into the stream, PUN will skip the update. Used carefully, this can
    /// conserve bandwidth and messages (which have a limit per room/second).
    ///
    /// Note that OnPhotonSerializeView is not called on remote clients when the sender does not send
    /// any update. This can't be used as "x-times per second Update()".
    /// </remarks>
    /// \ingroup publicApi
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info);
}

/// <summary>
/// This interface is used as definition of all callback methods of PUN, except OnPhotonSerializeView. Preferably, implement them individually.
/// </summary>
/// <remarks>
/// This interface is available for completeness, more than for actually implementing it in a game.
/// You can implement each method individually in any MonoMehaviour, without implementing IPunCallbacks.
///
/// PUN calls all callbacks by name. Don't use implement callbacks with fully qualified name.
/// Example: IPunCallbacks.OnConnectedToPhoton won't get called by Unity's SendMessage().
///
/// PUN will call these methods on any script that implements them, analog to Unity's events and callbacks.
/// The situation that triggers the call is described per method.
///
/// OnPhotonSerializeView is NOT called like these callbacks! It's usage frequency is much higher and it is implemented in: IPunObservable.
/// </remarks>
/// \ingroup publicApi
public interface IPunCallbacks
{
    /// <summary>
    /// Called when the initial connection got established but before you can use the server. OnJoinedLobby() or OnConnectedToMaster() are called when PUN is ready.
    /// </summary>
    /// <remarks>
    /// This callback is only useful to detect if the server can be reached at all (technically).
    /// Most often, it's enough to implement OnFailedToConnectToPhoton() and OnDisconnectedFromPhoton().
    ///
    /// <i>OnJoinedLobby() or OnConnectedToMaster() are called when PUN is ready.</i>
    ///
    /// When this is called, the low level connection is established and PUN will send your AppId, the user, etc in the background.
    /// This is not called for transitions from the masterserver to game servers.
    /// </remarks>
    void OnConnectedToPhoton();

    /// <summary>
    /// Called when the local user/client left a room.
    /// </summary>
    /// <remarks>
    /// When leaving a room, PUN brings you back to the Master Server.
    /// Before you can use lobbies and join or create rooms, OnJoinedLobby() or OnConnectedToMaster() will get called again.
    /// </remarks>
    void OnLeftRoom();

    /// <summary>
    /// Called after switching to a new MasterClient when the current one leaves.
    /// </summary>
    /// <remarks>
    /// This is not called when this client enters a room.
    /// The former MasterClient is still in the player list when this method get called.
    /// </remarks>
    void OnMasterClientSwitched(PhotonPlayer newMasterClient);

    /// <summary>
    /// Called when a CreateRoom() call failed. The parameter provides ErrorCode and message (as array).
    /// </summary>
    /// <remarks>
    /// Most likely because the room name is already in use (some other client was faster than you).
    /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
    /// </remarks>
    /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode and codeAndMsg[1] is a string debug msg.</param>
    void OnPhotonCreateRoomFailed(object[] codeAndMsg);

    /// <summary>
    /// Called when a JoinRoom() call failed. The parameter provides ErrorCode and message (as array).
    /// </summary>
    /// <remarks>
    /// Most likely error is that the room does not exist or the room is full (some other client was faster than you).
    /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
    /// </remarks>
    /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode and codeAndMsg[1] is string debug msg.</param>
    void OnPhotonJoinRoomFailed(object[] codeAndMsg);

    /// <summary>
    /// Called when this client created a room and entered it. OnJoinedRoom() will be called as well.
    /// </summary>
    /// <remarks>
    /// This callback is only called on the client which created a room (see PhotonNetwork.CreateRoom).
    ///
    /// As any client might close (or drop connection) anytime, there is a chance that the
    /// creator of a room does not execute OnCreatedRoom.
    ///
    /// If you need specific room properties or a "start signal", it is safer to implement
    /// OnMasterClientSwitched() and to make the new MasterClient check the room's state.
    /// </remarks>
    void OnCreatedRoom();

    /// <summary>
    /// Called on entering a lobby on the Master Server. The actual room-list updates will call OnReceivedRoomListUpdate().
    /// </summary>
    /// <remarks>
    /// Note: When PhotonNetwork.autoJoinLobby is false, OnConnectedToMaster() will be called and the room list won't become available.
    ///
    /// While in the lobby, the roomlist is automatically updated in fixed intervals (which you can't modify).
    /// The room list gets available when OnReceivedRoomListUpdate() gets called after OnJoinedLobby().
    /// </remarks>
    void OnJoinedLobby();

    /// <summary>
    /// Called after leaving a lobby.
    /// </summary>
    /// <remarks>
    /// When you leave a lobby, [CreateRoom](@ref PhotonNetwork.CreateRoom) and [JoinRandomRoom](@ref PhotonNetwork.JoinRandomRoom)
    /// automatically refer to the default lobby.
    /// </remarks>
    void OnLeftLobby();

    /// <summary>
    /// Called if a connect call to the Photon server failed before the connection was established, followed by a call to OnDisconnectedFromPhoton().
    /// </summary>
    /// <remarks>
    /// This is called when no connection could be established at all.
    /// It differs from OnConnectionFail, which is called when an existing connection fails.
    /// </remarks>
    void OnFailedToConnectToPhoton(DisconnectCause cause);

    /// <summary>
    /// Called when something causes the connection to fail (after it was established), followed by a call to OnDisconnectedFromPhoton().
    /// </summary>
    /// <remarks>
    /// If the server could not be reached in the first place, OnFailedToConnectToPhoton is called instead.
    /// The reason for the error is provided as DisconnectCause.
    /// </remarks>
    void OnConnectionFail(DisconnectCause cause);

    /// <summary>
    /// Called after disconnecting from the Photon server.
    /// </summary>
    /// <remarks>
    /// In some cases, other callbacks are called before OnDisconnectedFromPhoton is called.
    /// Examples: OnConnectionFail() and OnFailedToConnectToPhoton().
    /// </remarks>
    void OnDisconnectedFromPhoton();

    /// <summary>
    /// Called on all scripts on a GameObject (and children) that have been Instantiated using PhotonNetwork.Instantiate.
    /// </summary>
    /// <remarks>
    /// PhotonMessageInfo parameter provides info about who created the object and when (based off PhotonNetworking.time).
    /// </remarks>
    void OnPhotonInstantiate(PhotonMessageInfo info);

    /// <summary>
    /// Called for any update of the room-listing while in a lobby (PhotonNetwork.insideLobby) on the Master Server.
    /// </summary>
    /// <remarks>
    /// PUN provides the list of rooms by PhotonNetwork.GetRoomList().<br/>
    /// Each item is a RoomInfo which might include custom properties (provided you defined those as lobby-listed when creating a room).
    ///
    /// Not all types of lobbies provide a listing of rooms to the client. Some are silent and specialized for server-side matchmaking.
    /// </remarks>
    void OnReceivedRoomListUpdate();

    /// <summary>
    /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
    /// </summary>
    /// <remarks>
    /// This method is commonly used to instantiate player characters.
    /// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
    ///
    /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.playerList.
    /// Also, all custom properties should be already available as Room.customProperties. Check Room.playerCount to find out if
    /// enough players are in the room to start playing.
    /// </remarks>
    void OnJoinedRoom();

    /// <summary>
    /// Called when a remote player entered the room. This PhotonPlayer is already added to the playerlist at this time.
    /// </summary>
    /// <remarks>
    /// If your game starts with a certain number of players, this callback can be useful to check the
    /// Room.playerCount and find out if you can start.
    /// </remarks>
    void OnPhotonPlayerConnected(PhotonPlayer newPlayer);

    /// <summary>
    /// Called when a remote player left the room. This PhotonPlayer is already removed from the playerlist at this time.
    /// </summary>
    /// <remarks>
    /// When your client calls PhotonNetwork.leaveRoom, PUN will call this method on the remaining clients.
    /// When a remote client drops connection or gets closed, this callback gets executed. after a timeout
    /// of several seconds.
    /// </remarks>
    void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer);

    /// <summary>
    /// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
    /// </summary>
    /// <remarks>
    /// Most likely all rooms are full or no rooms are available. <br/>
    /// When using multiple lobbies (via JoinLobby or TypedLobby), another lobby might have more/fitting rooms.<br/>
    /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
    /// </remarks>
    /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.</param>
    void OnPhotonRandomJoinFailed(object[] codeAndMsg);

    /// <summary>
    /// Called after the connection to the master is established and authenticated but only when PhotonNetwork.autoJoinLobby is false.
    /// </summary>
    /// <remarks>
    /// If you set PhotonNetwork.autoJoinLobby to true, OnJoinedLobby() will be called instead of this.
    ///
    /// You can join rooms and create them even without being in a lobby. The default lobby is used in that case.
    /// The list of available rooms won't become available unless you join a lobby via PhotonNetwork.joinLobby.
    /// </remarks>
    void OnConnectedToMaster();

    /// <summary>
    /// Because the concurrent user limit was (temporarily) reached, this client is rejected by the server and disconnecting.
    /// </summary>
    /// <remarks>
    /// When this happens, the user might try again later. You can't create or join rooms in OnPhotonMaxCcuReached(), cause the client will be disconnecting.
    /// You can raise the CCU limits with a new license (when you host yourself) or extended subscription (when using the Photon Cloud).
    /// The Photon Cloud will mail you when the CCU limit was reached. This is also visible in the Dashboard (webpage).
    /// </remarks>
    void OnPhotonMaxCccuReached();

    /// <summary>
    /// Called when a room's custom properties changed. The propertiesThatChanged contains all that was set via Room.SetCustomProperties.
    /// </summary>
    /// <remarks>
    /// Since v1.25 this method has one parameter: Hashtable propertiesThatChanged.<br/>
    /// Changing properties must be done by Room.SetCustomProperties, which causes this callback locally, too.
    /// </remarks>
    /// <param name="propertiesThatChanged"></param>
    void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged);

    /// <summary>
    /// Called when custom player-properties are changed. Player and the changed properties are passed as object[].
    /// </summary>
    /// <remarks>
    /// Since v1.25 this method has one parameter: object[] playerAndUpdatedProps, which contains two entries.<br/>
    /// [0] is the affected PhotonPlayer.<br/>
    /// [1] is the Hashtable of properties that changed.<br/>
    ///
    /// We are using a object[] due to limitations of Unity's GameObject.SendMessage (which has only one optional parameter).
    ///
    /// Changing properties must be done by PhotonPlayer.SetCustomProperties, which causes this callback locally, too.
    ///
    /// Example:<pre>
    /// void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps) {
    ///     PhotonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;
    ///     Hashtable props = playerAndUpdatedProps[1] as Hashtable;
    ///     //...
    /// }</pre>
    /// </remarks>
    /// <param name="playerAndUpdatedProps">Contains PhotonPlayer and the properties that changed See remarks.</param>
    void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps);

    /// <summary>
    /// Called when the server sent the response to a FindFriends request and updated PhotonNetwork.Friends.
    /// </summary>
    /// <remarks>
    /// The friends list is available as PhotonNetwork.Friends, listing name, online state and
    /// the room a user is in (if any).
    /// </remarks>
    void OnUpdatedFriendList();

    /// <summary>
    /// Called when the custom authentication failed. Followed by disconnect!
    /// </summary>
    /// <remarks>
    /// Custom Authentication can fail due to user-input, bad tokens/secrets.
    /// If authentication is successful, this method is not called. Implement OnJoinedLobby() or OnConnectedToMaster() (as usual).
    ///
    /// During development of a game, it might also fail due to wrong configuration on the server side.
    /// In those cases, logging the debugMessage is very important.
    ///
    /// Unless you setup a custom authentication service for your app (in the [Dashboard](https://www.exitgames.com/dashboard)),
    /// this won't be called!
    /// </remarks>
    /// <param name="debugMessage">Contains a debug message why authentication failed. This has to be fixed during development time.</param>
    void OnCustomAuthenticationFailed(string debugMessage);

    /// <summary>
    /// Called by PUN when the response to a WebRPC is available. See PhotonNetwork.WebRPC.
    /// </summary>
    /// <remarks>
    /// Important: The response.ReturnCode is 0 if Photon was able to reach your web-service.<br/>
    /// The content of the response is what your web-service sent. You can create a WebRpcResponse from it.<br/>
    /// Example: WebRpcResponse webResponse = new WebRpcResponse(operationResponse);<br/>
    ///
    /// Please note: Class OperationResponse is in a namespace which needs to be "used":<br/>
    /// using ExitGames.Client.Photon;  // includes OperationResponse (and other classes)
    ///
    /// The OperationResponse.ReturnCode by Photon is:<pre>
    ///  0 for "OK"
    /// -3 for "Web-Service not configured" (see Dashboard / WebHooks)
    /// -5 for "Web-Service does now have RPC path/name" (at least for Azure)</pre>
    /// </remarks>
    void OnWebRpcResponse(OperationResponse response);

    /// <summary>
    /// Called when another player requests ownership of a PhotonView from you (the current owner).
    /// </summary>
    /// <remarks>
    /// The parameter viewAndPlayer contains:
    ///
    /// PhotonView view = viewAndPlayer[0] as PhotonView;
    ///
    /// PhotonPlayer requestingPlayer = viewAndPlayer[1] as PhotonPlayer;
    /// </remarks>
    /// <param name="viewAndPlayer">The PhotonView is viewAndPlayer[0] and the requesting player is viewAndPlayer[1].</param>
    void OnOwnershipRequest(object[] viewAndPlayer);

    /// <summary>
    /// Called when the Master Server sent an update for the Lobby Statistics, updating PhotonNetwork.LobbyStatistics.
    /// </summary>
    /// <remarks>
    /// This callback has two preconditions:
    /// EnableLobbyStatistics must be set to true, before this client connects.
    /// And the client has to be connected to the Master Server, which is providing the info about lobbies.
    /// </remarks>
    void OnLobbyStatisticsUpdate();
}

/// <summary>
/// Defines all the methods that a Object Pool must implement, so that PUN can use it.
/// </summary>
/// <remarks>
/// To use a Object Pool for instantiation, you can set PhotonNetwork.ObjectPool.
/// That is used for all objects, as long as ObjectPool is not null.
/// The pool has to return a valid non-null GameObject when PUN calls Instantiate.
/// Also, the position and rotation must be applied.
///
/// Please note that pooled GameObjects don't get the usual Awake and Start calls.
/// OnEnable will be called (by your pool) but the networking values are not updated yet
/// when that happens. OnEnable will have outdated values for PhotonView (isMine, etc.).
/// You might have to adjust scripts.
///
/// PUN will call OnPhotonInstantiate (see IPunCallbacks). This should be used to
/// setup the re-used object with regards to networking values / ownership.
/// </remarks>
public interface IPunPrefabPool
{
    /// <summary>
    /// This is called when PUN wants to create a new instance of an entity prefab. Must return valid GameObject with PhotonView.
    /// </summary>
    /// <param name="prefabId">The id of this prefab.</param>
    /// <param name="position">The position we want the instance instantiated at.</param>
    /// <param name="rotation">The rotation we want the instance to take.</param>
    /// <returns>The newly instantiated object, or null if a prefab with <paramref name="prefabId"/> was not found.</returns>
    GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation);

    /// <summary>
    /// This is called when PUN wants to destroy the instance of an entity prefab.
    /// </summary>
	/// <remarks>
	/// A pool needs some way to find out which type of GameObject got returned via Destroy().
	/// It could be a tag or name or anything similar.
	/// </remarks>
    /// <param name="gameObject">The instance to destroy.</param>
    void Destroy(GameObject gameObject);
}


namespace Photon
{
    using Hashtable = ExitGames.Client.Photon.Hashtable;

    /// <summary>
    /// This class adds the property photonView, while logging a warning when your game still uses the networkView.
    /// </summary>
    public class MonoBehaviour : UnityEngine.MonoBehaviour
    {
        public PhotonView photonView
        {
            get
            {
                return PhotonView.Get(this);
            }
        }

        /// <summary>
        /// This property is only here to notify developers when they use the outdated value.
        /// </summary>
        /// <remarks>
        /// If Unity 5.x logs a compiler warning "Use the new keyword if hiding was intended" or
        /// "The new keyword is not required", you may suffer from an Editor issue.
        /// Try to modify networkView with a if-def condition:
        ///
        /// #if UNITY_EDITOR
        /// new
        /// #endif
        /// public PhotonView networkView
        /// </remarks>
        new public PhotonView networkView
        {
            get
            {
                Debug.LogWarning("Why are you still using networkView? should be PhotonView?");
                return PhotonView.Get(this);
            }
        }
    }


    /// <summary>
    /// This class provides a .photonView and all callbacks/events that PUN can call. Override the events/methods you want to use.
    /// </summary>
    /// <remarks>
    /// By extending this class, you can implement individual methods as override.
    ///
    /// Visual Studio and MonoDevelop should provide the list of methods when you begin typing "override".
    /// <b>Your implementation does not have to call "base.method()".</b>
    ///
    /// This class implements IPunCallbacks, which is used as definition of all PUN callbacks.
    /// Don't implement IPunCallbacks in your classes. Instead, implent PunBehaviour or individual methods.
    /// </remarks>
    /// \ingroup publicApi
    // the documentation for the interface methods becomes inherited when Doxygen builds it.
    public class PunBehaviour : Photon.MonoBehaviour, IPunCallbacks
    {
        /// <summary>
        /// Called when the initial connection got established but before you can use the server. OnJoinedLobby() or OnConnectedToMaster() are called when PUN is ready.
        /// </summary>
        /// <remarks>
        /// This callback is only useful to detect if the server can be reached at all (technically).
        /// Most often, it's enough to implement OnFailedToConnectToPhoton() and OnDisconnectedFromPhoton().
        ///
        /// <i>OnJoinedLobby() or OnConnectedToMaster() are called when PUN is ready.</i>
        ///
        /// When this is called, the low level connection is established and PUN will send your AppId, the user, etc in the background.
        /// This is not called for transitions from the masterserver to game servers.
        /// </remarks>
        public virtual void OnConnectedToPhoton()
        {
        }

        /// <summary>
        /// Called when the local user/client left a room.
        /// </summary>
        /// <remarks>
        /// When leaving a room, PUN brings you back to the Master Server.
        /// Before you can use lobbies and join or create rooms, OnJoinedLobby() or OnConnectedToMaster() will get called again.
        /// </remarks>
        public virtual void OnLeftRoom()
        {
        }

        /// <summary>
        /// Called after switching to a new MasterClient when the current one leaves.
        /// </summary>
        /// <remarks>
        /// This is not called when this client enters a room.
        /// The former MasterClient is still in the player list when this method get called.
        /// </remarks>
        public virtual void OnMasterClientSwitched(PhotonPlayer newMasterClient)
        {
        }

        /// <summary>
        /// Called when a CreateRoom() call failed. The parameter provides ErrorCode and message (as array).
        /// </summary>
        /// <remarks>
        /// Most likely because the room name is already in use (some other client was faster than you).
        /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
        /// </remarks>
        /// <param name="codeAndMsg">codeAndMsg[0] is a short ErrorCode and codeAndMsg[1] is a string debug msg.</param>
        public virtual void OnPhotonCreateRoomFailed(object[] codeAndMsg)
        {
        }

        /// <summary>
        /// Called when a JoinRoom() call failed. The parameter provides ErrorCode and message (as array).
        /// </summary>
        /// <remarks>
        /// Most likely error is that the room does not exist or the room is full (some other client was faster than you).
        /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
        /// </remarks>
        /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.</param>
        public virtual void OnPhotonJoinRoomFailed(object[] codeAndMsg)
        {
        }

        /// <summary>
        /// Called when this client created a room and entered it. OnJoinedRoom() will be called as well.
        /// </summary>
        /// <remarks>
        /// This callback is only called on the client which created a room (see PhotonNetwork.CreateRoom).
        ///
        /// As any client might close (or drop connection) anytime, there is a chance that the
        /// creator of a room does not execute OnCreatedRoom.
        ///
        /// If you need specific room properties or a "start signal", it is safer to implement
        /// OnMasterClientSwitched() and to make the new MasterClient check the room's state.
        /// </remarks>
        public virtual void OnCreatedRoom()
        {
        }

        /// <summary>
        /// Called on entering a lobby on the Master Server. The actual room-list updates will call OnReceivedRoomListUpdate().
        /// </summary>
        /// <remarks>
        /// Note: When PhotonNetwork.autoJoinLobby is false, OnConnectedToMaster() will be called and the room list won't become available.
        ///
        /// While in the lobby, the roomlist is automatically updated in fixed intervals (which you can't modify).
        /// The room list gets available when OnReceivedRoomListUpdate() gets called after OnJoinedLobby().
        /// </remarks>
        public virtual void OnJoinedLobby()
        {
        }

        /// <summary>
        /// Called after leaving a lobby.
        /// </summary>
        /// <remarks>
        /// When you leave a lobby, [CreateRoom](@ref PhotonNetwork.CreateRoom) and [JoinRandomRoom](@ref PhotonNetwork.JoinRandomRoom)
        /// automatically refer to the default lobby.
        /// </remarks>
        public virtual void OnLeftLobby()
        {
        }

        /// <summary>
        /// Called if a connect call to the Photon server failed before the connection was established, followed by a call to OnDisconnectedFromPhoton().
        /// </summary>
        /// <remarks>
        /// This is called when no connection could be established at all.
        /// It differs from OnConnectionFail, which is called when an existing connection fails.
        /// </remarks>
        public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
        }

        /// <summary>
        /// Called after disconnecting from the Photon server.
        /// </summary>
        /// <remarks>
        /// In some cases, other callbacks are called before OnDisconnectedFromPhoton is called.
        /// Examples: OnConnectionFail() and OnFailedToConnectToPhoton().
        /// </remarks>
        public virtual void OnDisconnectedFromPhoton()
        {
        }

        /// <summary>
        /// Called when something causes the connection to fail (after it was established), followed by a call to OnDisconnectedFromPhoton().
        /// </summary>
        /// <remarks>
        /// If the server could not be reached in the first place, OnFailedToConnectToPhoton is called instead.
        /// The reason for the error is provided as DisconnectCause.
        /// </remarks>
        public virtual void OnConnectionFail(DisconnectCause cause)
        {
        }

        /// <summary>
        /// Called on all scripts on a GameObject (and children) that have been Instantiated using PhotonNetwork.Instantiate.
        /// </summary>
        /// <remarks>
        /// PhotonMessageInfo parameter provides info about who created the object and when (based off PhotonNetworking.time).
        /// </remarks>
        public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
        {
        }

        /// <summary>
        /// Called for any update of the room-listing while in a lobby (PhotonNetwork.insideLobby) on the Master Server.
        /// </summary>
        /// <remarks>
        /// PUN provides the list of rooms by PhotonNetwork.GetRoomList().<br/>
        /// Each item is a RoomInfo which might include custom properties (provided you defined those as lobby-listed when creating a room).
        ///
        /// Not all types of lobbies provide a listing of rooms to the client. Some are silent and specialized for server-side matchmaking.
        /// </remarks>
        public virtual void OnReceivedRoomListUpdate()
        {
        }

        /// <summary>
        /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
        /// </summary>
        /// <remarks>
        /// This method is commonly used to instantiate player characters.
        /// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
        ///
        /// When this is called, you can usually already access the existing players in the room via PhotonNetwork.playerList.
        /// Also, all custom properties should be already available as Room.customProperties. Check Room.playerCount to find out if
        /// enough players are in the room to start playing.
        /// </remarks>
        public virtual void OnJoinedRoom()
        {
        }

        /// <summary>
        /// Called when a remote player entered the room. This PhotonPlayer is already added to the playerlist at this time.
        /// </summary>
        /// <remarks>
        /// If your game starts with a certain number of players, this callback can be useful to check the
        /// Room.playerCount and find out if you can start.
        /// </remarks>
        public virtual void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
        }

        /// <summary>
        /// Called when a remote player left the room. This PhotonPlayer is already removed from the playerlist at this time.
        /// </summary>
        /// <remarks>
        /// When your client calls PhotonNetwork.leaveRoom, PUN will call this method on the remaining clients.
        /// When a remote client drops connection or gets closed, this callback gets executed. after a timeout
        /// of several seconds.
        /// </remarks>
        public virtual void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
        }

        /// <summary>
        /// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
        /// </summary>
        /// <remarks>
        /// Most likely all rooms are full or no rooms are available. <br/>
        /// When using multiple lobbies (via JoinLobby or TypedLobby), another lobby might have more/fitting rooms.<br/>
        /// PUN logs some info if the PhotonNetwork.logLevel is >= PhotonLogLevel.Informational.
        /// </remarks>
        /// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.</param>
        public virtual void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
        }

        /// <summary>
        /// Called after the connection to the master is established and authenticated but only when PhotonNetwork.autoJoinLobby is false.
        /// </summary>
        /// <remarks>
        /// If you set PhotonNetwork.autoJoinLobby to true, OnJoinedLobby() will be called instead of this.
        ///
        /// You can join rooms and create them even without being in a lobby. The default lobby is used in that case.
        /// The list of available rooms won't become available unless you join a lobby via PhotonNetwork.joinLobby.
        /// </remarks>
        public virtual void OnConnectedToMaster()
        {
        }

        /// <summary>
        /// Because the concurrent user limit was (temporarily) reached, this client is rejected by the server and disconnecting.
        /// </summary>
        /// <remarks>
        /// When this happens, the user might try again later. You can't create or join rooms in OnPhotonMaxCcuReached(), cause the client will be disconnecting.
        /// You can raise the CCU limits with a new license (when you host yourself) or extended subscription (when using the Photon Cloud).
        /// The Photon Cloud will mail you when the CCU limit was reached. This is also visible in the Dashboard (webpage).
        /// </remarks>
        public virtual void OnPhotonMaxCccuReached()
        {
        }

        /// <summary>
        /// Called when a room's custom properties changed. The propertiesThatChanged contains all that was set via Room.SetCustomProperties.
        /// </summary>
        /// <remarks>
        /// Since v1.25 this method has one parameter: Hashtable propertiesThatChanged.<br/>
        /// Changing properties must be done by Room.SetCustomProperties, which causes this callback locally, too.
        /// </remarks>
        /// <param name="propertiesThatChanged"></param>
        public virtual void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
        {
        }

        /// <summary>
        /// Called when custom player-properties are changed. Player and the changed properties are passed as object[].
        /// </summary>
        /// <remarks>
        /// Since v1.25 this method has one parameter: object[] playerAndUpdatedProps, which contains two entries.<br/>
        /// [0] is the affected PhotonPlayer.<br/>
        /// [1] is the Hashtable of properties that changed.<br/>
        ///
        /// We are using a object[] due to limitations of Unity's GameObject.SendMessage (which has only one optional parameter).
        ///
        /// Changing properties must be done by PhotonPlayer.SetCustomProperties, which causes this callback locally, too.
        ///
        /// Example:<pre>
        /// void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps) {
        ///     PhotonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;
        ///     Hashtable props = playerAndUpdatedProps[1] as Hashtable;
        ///     //...
        /// }</pre>
        /// </remarks>
        /// <param name="playerAndUpdatedProps">Contains PhotonPlayer and the properties that changed See remarks.</param>
        public virtual void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
        {
        }

        /// <summary>
        /// Called when the server sent the response to a FindFriends request and updated PhotonNetwork.Friends.
        /// </summary>
        /// <remarks>
        /// The friends list is available as PhotonNetwork.Friends, listing name, online state and
        /// the room a user is in (if any).
        /// </remarks>
        public virtual void OnUpdatedFriendList()
        {
        }

        /// <summary>
        /// Called when the custom authentication failed. Followed by disconnect!
        /// </summary>
        /// <remarks>
        /// Custom Authentication can fail due to user-input, bad tokens/secrets.
        /// If authentication is successful, this method is not called. Implement OnJoinedLobby() or OnConnectedToMaster() (as usual).
        ///
        /// During development of a game, it might also fail due to wrong configuration on the server side.
        /// In those cases, logging the debugMessage is very important.
        ///
        /// Unless you setup a custom authentication service for your app (in the [Dashboard](https://www.exitgames.com/dashboard)),
        /// this won't be called!
        /// </remarks>
        /// <param name="debugMessage">Contains a debug message why authentication failed. This has to be fixed during development time.</param>
        public virtual void OnCustomAuthenticationFailed(string debugMessage)
        {
        }

        /// <summary>
        /// Called by PUN when the response to a WebRPC is available. See PhotonNetwork.WebRPC.
        /// </summary>
        /// <remarks>
        /// Important: The response.ReturnCode is 0 if Photon was able to reach your web-service.
        /// The content of the response is what your web-service sent. You can create a WebResponse instance from it.
        /// Example: WebRpcResponse webResponse = new WebRpcResponse(operationResponse);
        ///
        /// Please note: Class OperationResponse is in a namespace which needs to be "used":
        /// using ExitGames.Client.Photon;  // includes OperationResponse (and other classes)
        ///
        /// The OperationResponse.ReturnCode by Photon is:<pre>
        ///  0 for "OK"
        /// -3 for "Web-Service not configured" (see Dashboard / WebHooks)
        /// -5 for "Web-Service does now have RPC path/name" (at least for Azure)</pre>
        /// </remarks>
        public virtual void OnWebRpcResponse(OperationResponse response)
        {
        }

        /// <summary>
        /// Called when another player requests ownership of a PhotonView from you (the current owner).
        /// </summary>
        /// <remarks>
        /// The parameter viewAndPlayer contains:
        ///
        /// PhotonView view = viewAndPlayer[0] as PhotonView;
        ///
        /// PhotonPlayer requestingPlayer = viewAndPlayer[1] as PhotonPlayer;
        /// </remarks>
        /// <param name="viewAndPlayer">The PhotonView is viewAndPlayer[0] and the requesting player is viewAndPlayer[1].</param>
        public virtual void OnOwnershipRequest(object[] viewAndPlayer)
        {
        }

        /// <summary>
        /// Called when the Master Server sent an update for the Lobby Statistics, updating PhotonNetwork.LobbyStatistics.
        /// </summary>
        /// <remarks>
        /// This callback has two preconditions:
        /// EnableLobbyStatistics must be set to true, before this client connects.
        /// And the client has to be connected to the Master Server, which is providing the info about lobbies.
        /// </remarks>
        public virtual void OnLobbyStatisticsUpdate()
        {
        }
    }
}


/// <summary>
/// Container class for info about a particular message, RPC or update.
/// </summary>
/// \ingroup publicApi
public class PhotonMessageInfo
{
    private int timeInt;
    public PhotonPlayer sender;
    public PhotonView photonView;

    /// <summary>
    /// Initializes a new instance of the <see cref="PhotonMessageInfo"/> class.
    /// To create an empty messageinfo only!
    /// </summary>
    public PhotonMessageInfo()
    {
        this.sender = PhotonNetwork.player;
        this.timeInt = (int)(PhotonNetwork.time * 1000);
        this.photonView = null;
    }

    public PhotonMessageInfo(PhotonPlayer player, int timestamp, PhotonView view)
    {
        this.sender = player;
        this.timeInt = timestamp;
        this.photonView = view;
    }

    public double timestamp
    {
        get
        {
            uint u = (uint)this.timeInt;
            double t = u;
            return t / 1000;
        }
    }

    public override string ToString()
    {
        return string.Format("[PhotonMessageInfo: Sender='{1}' Senttime={0}]", this.timestamp, this.sender);
    }
}

/// <summary>Wraps up common room properties needed when you create rooms.</summary>
/// <remarks>This directly maps to what the fields in the Room class.</remarks>
public class RoomOptions
{
    /// <summary>Defines if this room is listed in the lobby. If not, it also is not joined randomly.</summary>
    /// <remarks>
    /// A room that is not visible will be excluded from the room lists that are sent to the clients in lobbies.
    /// An invisible room can be joined by name but is excluded from random matchmaking.
    ///
    /// Use this to "hide" a room and simulate "private rooms". Players can exchange a roomname and create it
    /// invisble to avoid anyone else joining it.
    /// </remarks>
    public bool isVisible { get { return this.isVisibleField; } set { this.isVisibleField = value; } }
    private bool isVisibleField = true;

    /// <summary>Defines if this room can be joined at all.</summary>
    /// <remarks>
    /// If a room is closed, no player can join this. As example this makes sense when 3 of 4 possible players
    /// start their gameplay early and don't want anyone to join during the game.
    /// The room can still be listed in the lobby (set isVisible to control lobby-visibility).
    /// </remarks>
    public bool isOpen { get { return this.isOpenField; } set { this.isOpenField = value; } }
    private bool isOpenField = true;

    /// <summary>Max number of players that can be in the room at any time. 0 means "no limit".</summary>
    public byte maxPlayers;

    /// <summary>Time To Live (TTL) for an 'actor' in a room. If a client disconnects, this actor is inactive first and removed after this timeout. In milliseconds.</summary>
    // public int PlayerTtl;

    /// <summary>Time To Live (TTL) for a room when the last player leaves. Keeps room in memory for case a player re-joins soon. In milliseconds.</summary>
    // public int EmptyRoomTtl;


    /// <summary>Time To Live (TTL) for a room when the last player leaves. Keeps room in memory for case a player re-joins soon. In milliseconds.</summary>
    //public int EmptyRoomTtl;

    ///// <summary>Activates UserId checks on joining - allowing a users to be only once in the room.</summary>
    ///// <remarks>
    ///// Turnbased rooms should be created with this check turned on! They should also use custom authentication.
    ///// Disabled by default for backwards-compatibility.
    ///// </remarks>
    //public bool checkUserOnJoin { get { return this.checkUserOnJoinField; } set { this.checkUserOnJoinField = value; } }
    //private bool checkUserOnJoinField = false;

    /// <summary>Removes a user's events and properties from the room when a user leaves.</summary>
    /// <remarks>
    /// This makes sense when in rooms where players can't place items in the room and just vanish entirely.
    /// When you disable this, the event history can become too long to load if the room stays in use indefinitely.
    /// Default: true. Cleans up the cache and props of leaving users.
    /// </remarks>
    public bool cleanupCacheOnLeave { get { return this.cleanupCacheOnLeaveField; } set { this.cleanupCacheOnLeaveField = value; } }
    private bool cleanupCacheOnLeaveField = PhotonNetwork.autoCleanUpPlayerObjects;

    /// <summary>The room's custom properties to set. Use string keys!</summary>
    /// <remarks>
    /// Custom room properties are any key-values you need to define the game's setup.
    /// The shorter your keys are, the better.
    /// Example: Map, Mode (could be "m" when used with "Map"), TileSet (could be "t").
    /// </remarks>
    public Hashtable customRoomProperties;

    /// <summary>Defines the custom room properties that get listed in the lobby.</summary>
    /// <remarks>
    /// Name the custom room properties that should be available to clients that are in a lobby.
    /// Use with care. Unless a custom property is essential for matchmaking or user info, it should
    /// not be sent to the lobby, which causes traffic and delays for clients in the lobby.
    ///
    /// Default: No custom properties are sent to the lobby.
    /// </remarks>
    public string[] customRoomPropertiesForLobby = new string[0];

    /// <summary>
    /// Tells the server to skip room events for joining and leaving players.
    /// </summary>
    /// <remarks>
    /// Using this makes the client unaware of the other players in a room.
    /// That can save some traffic if you have some server logic that updates players
    /// but it can also limit the client's usability.
    ///
    /// PUN will break if you use this, so it's not settable.
    /// </remarks>
    public bool suppressRoomEvents { get { return this.suppressRoomEventsField; } /*set { this.suppressRoomEventsField = value; }*/ }
    private bool suppressRoomEventsField = false;


    ///// <summary>
    ///// Defines if the UserIds of players get "published" in the room. Useful for FindFriends, if players want to play another game together.
    ///// </summary>
    //public bool publishUserId { get { return this.publishUserIdField; } set { this.publishUserIdField = value; } }
    //private bool publishUserIdField = false;
}


///// <summary>Refers to a specific lobby (and type) on the server.</summary>
///// <remarks>
///// The name and type are the unique identifier for a lobby.<br/>
///// Join a lobby via PhotonNetwork.JoinLobby(TypedLobby lobby).<br/>
///// The current lobby is stored in PhotonNetwork.lobby.
///// </remarks>
//public class TypedLobby
//{
//    /// <summary>
//    /// The name of the Lobby. Can be any string. Default lobby uses "".
//    /// </summary>
//    public string Name;

//    /// <summary>
//    /// The type of the Lobby. Default lobby uses LobbyType.Default.
//    /// </summary>
//    public LobbyType Type;

//    public static readonly TypedLobby Default = new TypedLobby();
//    public bool IsDefault { get { return this.Type == LobbyType.Default && string.IsNullOrEmpty(this.Name); } }

//    public TypedLobby()
//    {
//        this.Name = string.Empty;
//        this.Type = LobbyType.Default;
//    }

//    public TypedLobby(string name, LobbyType type)
//    {
//        this.Name = name;
//        this.Type = type;
//    }

//    public override string ToString()
//    {
//        return string.Format("Lobby '{0}'[{1}]", this.Name, this.Type);
//    }
//}


///// <summary>Used in the PhotonNetwork.LobbyStatistics list of lobbies used by your application. Contains room- and player-count for each.</summary>
//public class TypedLobbyInfo : TypedLobby
//{
//    public int PlayerCount;
//    public int RoomCount;

//    public override string ToString()
//    {
//        return string.Format("LobbyInfo '{0}'[{1}] rooms: {2} players: {3}", this.Name, this.Type, this.RoomCount, this.PlayerCount);
//    }
//}


///// <summary>Aggregates several less-often used options for operation RaiseEvent. See field descriptions for usage details.</summary>
//public class RaiseEventOptions
//{
//    /// <summary>Default options: CachingOption: DoNotCache, InterestGroup: 0, targetActors: null, receivers: Others, sequenceChannel: 0.</summary>
//    public readonly static RaiseEventOptions Default = new RaiseEventOptions();

//    /// <summary>Defines if the server should simply send the event, put it in the cache or remove events that are like this one.</summary>
//    /// <remarks>
//    /// When using option: SliceSetIndex, SlicePurgeIndex or SlicePurgeUpToIndex, set a CacheSliceIndex. All other options except SequenceChannel get ignored.
//    /// </remarks>
//    public EventCaching CachingOption;

//    /// <summary>The number of the Interest Group to send this to. 0 goes to all users but to get 1 and up, clients must subscribe to the group first.</summary>
//    public byte InterestGroup;

//    /// <summary>A list of PhotonPlayer.IDs to send this event to. You can implement events that just go to specific users this way.</summary>
//    public int[] TargetActors;

//    /// <summary>Sends the event to All, MasterClient or Others (default). Be careful with MasterClient, as the client might disconnect before it got the event and it gets lost.</summary>
//    public ReceiverGroup Receivers;

//    /// <summary>Events are ordered per "channel". If you have events that are independent of others, they can go into another sequence or channel.</summary>
//    public byte SequenceChannel;

//    /// <summary>Events can be forwarded to Webhooks, which can evaluate and use the events to follow the game's state.</summary>
//    public bool ForwardToWebhook;

//    /// <summary>Used along with CachingOption SliceSetIndex, SlicePurgeIndex or SlicePurgeUpToIndex if you want to set or purge a specific cache-slice.</summary>
//    public int CacheSliceIndex;

//    /// <summary>Use rarely. The binary message gets encrpted before being sent. Any receiver in the room will be able to decrypt the message, of course.</summary>
//    public bool Encrypt;
//}

/// <summary>Defines Photon event-codes as used by PUN.</summary>
internal class PunEvent
{
    public const byte RPC = 200;
    public const byte SendSerialize = 201;
    public const byte Instantiation = 202;
    public const byte CloseConnection = 203;
    public const byte Destroy = 204;
    public const byte RemoveCachedRPCs = 205;
    public const byte SendSerializeReliable = 206;  // TS: added this but it's not really needed anymore
    public const byte DestroyPlayer = 207;  // TS: added to make others remove all GOs of a player
    public const byte AssignMaster = 208;  // TS: added to assign someone master client (overriding the current)
    public const byte OwnershipRequest = 209;
    public const byte OwnershipTransfer = 210;
    public const byte VacantViewIds = 211;
}

/// <summary>
/// This container is used in OnPhotonSerializeView() to either provide incoming data of a PhotonView or for you to provide it.
/// </summary>
/// <remarks>
/// The isWriting property will be true if this client is the "owner" of the PhotonView (and thus the GameObject).
/// Add data to the stream and it's sent via the server to the other players in a room.
/// On the receiving side, isWriting is false and the data should be read.
///
/// Send as few data as possible to keep connection quality up. An empty PhotonStream will not be sent.
///
/// Use either Serialize() for reading and writing or SendNext() and ReceiveNext(). The latter two are just explicit read and
/// write methods but do about the same work as Serialize(). It's a matter of preference which methods you use.
/// </remarks>
/// <seealso cref="PhotonNetworkingMessage"/>
/// \ingroup publicApi
public class PhotonStream
{
    bool write = false;
    internal List<object> data;
    byte currentItem = 0; //Used to track the next item to receive.

    /// <summary>
    /// Creates a stream and initializes it. Used by PUN internally.
    /// </summary>
    public PhotonStream(bool write, object[] incomingData)
    {
        this.write = write;
        if (incomingData == null)
        {
            this.data = new List<object>();
        }
        else
        {
            this.data = new List<object>(incomingData);
        }
    }

    /// <summary>If true, this client should add data to the stream to send it.</summary>
    public bool isWriting
    {
        get { return this.write; }
    }

    /// <summary>If true, this client should read data send by another client.</summary>
    public bool isReading
    {
        get { return !this.write; }
    }

    /// <summary>Count of items in the stream.</summary>
    public int Count
    {
        get
        {
            return data.Count;
        }
    }

    /// <summary>Read next piece of data from the stream when isReading is true.</summary>
    public object ReceiveNext()
    {
        if (this.write)
        {
            Debug.LogError("Error: you cannot read this stream that you are writing!");
            return null;
        }

        object obj = this.data[this.currentItem];
        this.currentItem++;
        return obj;
    }

    /// <summary>Read next piece of data from the stream without advancing the "current" item.</summary>
    public object PeekNext()
    {
        if (this.write)
        {
            Debug.LogError("Error: you cannot read this stream that you are writing!");
            return null;
        }

        object obj = this.data[this.currentItem];
        //this.currentItem++;
        return obj;
    }

    /// <summary>Add another piece of data to send it when isWriting is true.</summary>
    public void SendNext(object obj)
    {
        if (!this.write)
        {
            Debug.LogError("Error: you cannot write/send to this stream that you are reading!");
            return;
        }

        this.data.Add(obj);
    }

    /// <summary>Turns the stream into a new object[].</summary>
    public object[] ToArray()
    {
        return this.data.ToArray();
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref bool myBool)
    {
        if (this.write)
        {
            this.data.Add(myBool);
        }
        else
        {
            if (this.data.Count > currentItem)
            {
                myBool = (bool)data[currentItem];
                this.currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref int myInt)
    {
        if (write)
        {
            this.data.Add(myInt);
        }
        else
        {
            if (this.data.Count > currentItem)
            {
                myInt = (int)data[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref string value)
    {
        if (write)
        {
            this.data.Add(value);
        }
        else
        {
            if (this.data.Count > currentItem)
            {
                value = (string)data[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref char value)
    {
        if (write)
        {
            this.data.Add(value);
        }
        else
        {
            if (this.data.Count > currentItem)
            {
                value = (char)data[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref short value)
    {
        if (write)
        {
            this.data.Add(value);
        }
        else
        {
            if (this.data.Count > currentItem)
            {
                value = (short)data[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref float obj)
    {
        if (write)
        {
            this.data.Add(obj);
        }
        else
        {
            if (this.data.Count > currentItem)
            {
                obj = (float)data[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref PhotonPlayer obj)
    {
        if (write)
        {
            this.data.Add(obj);
        }
        else
        {
            if (this.data.Count > currentItem)
            {
                obj = (PhotonPlayer)data[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref Vector3 obj)
    {
        if (write)
        {
            this.data.Add(obj);
        }
        else
        {
            if (this.data.Count > currentItem)
            {
                obj = (Vector3)data[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref Vector2 obj)
    {
        if (write)
        {
            this.data.Add(obj);
        }
        else
        {
            if (this.data.Count > currentItem)
            {
                obj = (Vector2)data[currentItem];
                currentItem++;
            }
        }
    }

    /// <summary>
    /// Will read or write the value, depending on the stream's isWriting value.
    /// </summary>
    public void Serialize(ref Quaternion obj)
    {
        if (write)
        {
            this.data.Add(obj);
        }
        else
        {
            if (this.data.Count > currentItem)
            {
                obj = (Quaternion)data[currentItem];
                currentItem++;
            }
        }
    }
}


#if UNITY_5_0 || !UNITY_5
/// <summary>Empty implementation of the upcoming HelpURL of Unity 5.1. This one is only for compatibility of attributes.</summary>
/// <remarks>http://feedback.unity3d.com/suggestions/override-component-documentation-slash-help-link</remarks>
public class HelpURL : Attribute
{
    public HelpURL(string url)
    {
    }
}
#endif


#if !UNITY_MIN_5_3
// in Unity 5.3 and up, we have to use a SceneManager. This section re-implements it for older Unity versions

#if UNITY_EDITOR
namespace UnityEditor.SceneManagement
{
    /// <summary>Minimal implementation of the EditorSceneManager for older Unity, up to v5.2.</summary>
    public class EditorSceneManager
    {
        public static int loadedSceneCount
        {
            get { return string.IsNullOrEmpty(UnityEditor.EditorApplication.currentScene) ? -1 : 1; }
        }

        public static void OpenScene(string name)
        {
            UnityEditor.EditorApplication.OpenScene(name);
        }

        public static void SaveOpenScenes()
        {
            UnityEditor.EditorApplication.SaveScene();
        }

        public static void SaveCurrentModifiedScenesIfUserWantsTo()
        {
            UnityEditor.EditorApplication.SaveCurrentSceneIfUserWantsTo();
        }
    }
}
#endif

namespace UnityEngine.SceneManagement
{
    /// <summary>Minimal implementation of the SceneManager for older Unity, up to v5.2.</summary>
    public class SceneManager
    {
        public static void LoadScene(string name)
        {
            Application.LoadLevel(name);
        }

        public static void LoadScene(int buildIndex)
        {
            Application.LoadLevel(buildIndex);
        }
    }
}

#endif


public class SceneManagerHelper
{
    public static string ActiveSceneName
    {
        get
        {
            #if UNITY_MIN_5_3
            UnityEngine.SceneManagement.Scene s = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            return s.name;
            #else
            return Application.loadedLevelName;
            #endif
        }
    }

    public static int ActiveSceneBuildIndex
    {
        get
        {
            #if UNITY_MIN_5_3
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            #else
            return Application.loadedLevel;
            #endif
        }
    }


#if UNITY_EDITOR
    public static string EditorActiveSceneName
    {
        get
        {
            #if UNITY_MIN_5_3
            return UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
            #else
            return System.IO.Path.GetFileNameWithoutExtension(UnityEditor.EditorApplication.currentScene);
            #endif
        }
    }
#endif
}


/// <summary>Reads an operation response of a WebRpc and provides convenient access to most common values.</summary>
/// <remarks>
/// See method PhotonNetwork.WebRpc.<br/>
/// Create a WebRpcResponse to access common result values.<br/>
/// The operationResponse.OperationCode should be: OperationCode.WebRpc.<br/>
/// </remarks>
public class WebRpcResponse
{
    /// <summary>Name of the WebRpc that was called.</summary>
    public string Name { get; private set; }
    /// <summary>ReturnCode of the WebService that answered the WebRpc.</summary>
    /// <remarks>
    /// 0 is commonly used to signal success.<br/>
    /// -1 tells you: Got no ReturnCode from WebRpc service.<br/>
    /// Other ReturnCodes are defined by the individual WebRpc and service.
    /// </remarks>
    public int ReturnCode { get; private set; }
    /// <summary>Might be empty or null.</summary>
    public string DebugMessage { get; private set; }
    /// <summary>Other key/values returned by the webservice that answered the WebRpc.</summary>
    public Dictionary<string, object> Parameters { get; private set; }

    /// <summary>An OperationResponse for a WebRpc is needed to read it's values.</summary>
    public WebRpcResponse(OperationResponse response)
    {
        object value;
        response.Parameters.TryGetValue(ParameterCode.UriPath, out value);
        this.Name = value as string;

        response.Parameters.TryGetValue(ParameterCode.WebRpcReturnCode, out value);
        this.ReturnCode = (value != null) ? (byte)value : -1;

        response.Parameters.TryGetValue(ParameterCode.WebRpcParameters, out value);
        this.Parameters = value as Dictionary<string, object>;

        response.Parameters.TryGetValue(ParameterCode.WebRpcReturnMessage, out value);
        this.DebugMessage = value as string;
    }

    /// <summary>Turns the response into an easier to read string.</summary>
    /// <returns>String resembling the result.</returns>
    public string ToStringFull()
    {
        return string.Format("{0}={2}: {1} \"{3}\"", Name, SupportClass.DictionaryToString(Parameters), ReturnCode, DebugMessage);
    }
}

/**
public class PBitStream
{
    List<byte> streamBytes;
    private int currentByte;
    private int totalBits = 0;

    public int ByteCount
    {
        get { return BytesForBits(this.totalBits); }
    }

    public int BitCount
    {
        get { return this.totalBits; }
        private set { this.totalBits = value; }
    }

    public PBitStream()
    {
        this.streamBytes = new List<byte>(1);
    }

    public PBitStream(int bitCount)
    {
        this.streamBytes = new List<byte>(BytesForBits(bitCount));
    }

    public PBitStream(IEnumerable<byte> bytes, int bitCount)
    {
        this.streamBytes = new List<byte>(bytes);
        this.BitCount = bitCount;
    }

    public static int BytesForBits(int bitCount)
    {
        if (bitCount <= 0)
        {
            return 0;
        }

        return ((bitCount - 1) / 8) + 1;
    }

    public void Add(bool val)
    {
        int bytePos = this.totalBits / 8;
        if (bytePos > this.streamBytes.Count-1 || this.totalBits == 0)
        {
            this.streamBytes.Add(0);
        }

        if (val)
        {
            int currentByteBit = 7 - (this.totalBits % 8);
            this.streamBytes[bytePos] |= (byte)(1 << currentByteBit);
        }

        this.totalBits++;
    }

    public byte[] ToBytes()
    {
        return this.streamBytes.ToArray();
    }

    public int Position { get; set; }

    public bool GetNext()
    {
        if (this.Position > this.totalBits)
        {
            throw new Exception("End of PBitStream reached. Can't read more.");
        }

        return Get(this.Position++);
    }

    public bool Get(int bitIndex)
    {
        int byteIndex = bitIndex / 8;
        int bitInByIndex = 7 - (bitIndex % 8);
        return ((this.streamBytes[byteIndex] & (byte)(1 << bitInByIndex)) > 0);
    }

    public void Set(int bitIndex, bool value)
    {
        int byteIndex = bitIndex / 8;
        int bitInByIndex = 7 - (bitIndex % 8);
        this.streamBytes[byteIndex] |= (byte)(1 << bitInByIndex);
    }
}
**/
