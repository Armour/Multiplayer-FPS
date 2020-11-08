// ----------------------------------------------------------------------------
// <copyright file="PhotonHandler.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// PhotonHandler is a runtime MonoBehaviour to include PUN into the main loop.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using ExitGames.Client.Photon;
    using Photon.Realtime;
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_5_5_OR_NEWER
    using UnityEngine.Profiling;
#endif


    /// <summary>
    /// Internal MonoBehaviour that allows Photon to run an Update loop.
    /// </summary>
    public class PhotonHandler : ConnectionHandler, IInRoomCallbacks, IMatchmakingCallbacks
    {

        private static PhotonHandler instance;
        internal static PhotonHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PhotonHandler>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = "PhotonMono";
                        instance = obj.AddComponent<PhotonHandler>();
                    }
                }

                return instance;
            }
        }


        /// <summary>Limits the number of datagrams that are created in each LateUpdate.</summary>
        /// <remarks>Helps spreading out sending of messages minimally.</remarks>
        public static int MaxDatagrams = 10;

        /// <summary>Signals that outgoing messages should be sent in the next LateUpdate call.</summary>
        /// <remarks>Up to MaxDatagrams are created to send queued messages.</remarks>
        public static bool SendAsap;

        /// <summary>This corrects the "next time to serialize the state" value by some ms.</summary>
        /// <remarks>As LateUpdate typically gets called every 15ms it's better to be early(er) than late to achieve a SerializeRate.</remarks>
        private const int SerializeRateFrameCorrection = 8;

        protected internal int UpdateInterval; // time [ms] between consecutive SendOutgoingCommands calls

        protected internal int UpdateIntervalOnSerialize; // time [ms] between consecutive RunViewUpdate calls (sending syncs, etc)

        private int nextSendTickCount;

        private int nextSendTickCountOnSerialize;

        private SupportLogger supportLoggerComponent;


        protected override void Awake()
        {

            if (instance == null || ReferenceEquals(this, instance))
            {
                instance = this;
                base.Awake();
            }
            else
            {
                Destroy(this);
            }
        }

        protected virtual void OnEnable()
        {

            if (Instance != this)
            {
                Debug.LogError("PhotonHandler is a singleton but there are multiple instances. this != Instance.");
                return;
            }

            this.Client = PhotonNetwork.NetworkingClient;

            if (PhotonNetwork.PhotonServerSettings.EnableSupportLogger)
            {
                SupportLogger supportLogger = this.gameObject.GetComponent<SupportLogger>();
                if (supportLogger == null)
                {
                    supportLogger = this.gameObject.AddComponent<SupportLogger>();
                }
                if (this.supportLoggerComponent != null)
                {
                    if (supportLogger.GetInstanceID() != this.supportLoggerComponent.GetInstanceID())
                    {
                        Debug.LogWarningFormat("Cached SupportLogger component is different from the one attached to PhotonMono GameObject");
                    }
                }
                this.supportLoggerComponent = supportLogger;
                this.supportLoggerComponent.Client = PhotonNetwork.NetworkingClient;
            }

            this.UpdateInterval = 1000 / PhotonNetwork.SendRate;
            this.UpdateIntervalOnSerialize = 1000 / PhotonNetwork.SerializationRate;

            PhotonNetwork.AddCallbackTarget(this);
            this.StartFallbackSendAckThread();  // this is not done in the base class
        }

        protected void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
            {
                PhotonNetwork.NewSceneLoaded();
            };
        }

        protected override void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
            base.OnDisable();
        }


        /// <summary>Called in intervals by UnityEngine. Affected by Time.timeScale.</summary>
        protected void FixedUpdate()
        {
            this.Dispatch();
        }

        /// <summary>Called in intervals by UnityEngine, after running the normal game code and physics.</summary>
        protected void LateUpdate()
        {
            // see MinimalTimeScaleToDispatchInFixedUpdate and FixedUpdate for explanation:
            if (Time.timeScale <= PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate)
            {
                this.Dispatch();
            }


            int currentMsSinceStart = (int)(Time.realtimeSinceStartup * 1000); // avoiding Environment.TickCount, which could be negative on long-running platforms
            if (PhotonNetwork.IsMessageQueueRunning && currentMsSinceStart > this.nextSendTickCountOnSerialize)
            {
                PhotonNetwork.RunViewUpdate();
                this.nextSendTickCountOnSerialize = currentMsSinceStart + this.UpdateIntervalOnSerialize - SerializeRateFrameCorrection;
                this.nextSendTickCount = 0; // immediately send when synchronization code was running
            }

            currentMsSinceStart = (int)(Time.realtimeSinceStartup * 1000);
            if (SendAsap || currentMsSinceStart > this.nextSendTickCount)
            {
                SendAsap = false;
                bool doSend = true;
                int sendCounter = 0;
                while (PhotonNetwork.IsMessageQueueRunning && doSend && sendCounter < MaxDatagrams)
                {
                    // Send all outgoing commands
                    Profiler.BeginSample("SendOutgoingCommands");
                    doSend = PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendOutgoingCommands();
                    sendCounter++;
                    Profiler.EndSample();
                }

                this.nextSendTickCount = currentMsSinceStart + this.UpdateInterval;
            }
        }

        /// <summary>Dispatches incoming network messages for PUN. Called in FixedUpdate or LateUpdate.</summary>
        /// <remarks>
        /// It may make sense to dispatch incoming messages, even if the timeScale is near 0.
        /// That can be configured with PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate.
        ///
        /// Without dispatching messages, PUN won't change state and does not handle updates.
        /// </remarks>
        protected void Dispatch()
        {
            if (PhotonNetwork.NetworkingClient == null)
            {
                Debug.LogError("NetworkPeer broke!");
                return;
            }

            //if (PhotonNetwork.NetworkClientState == ClientState.PeerCreated || PhotonNetwork.NetworkClientState == ClientState.Disconnected || PhotonNetwork.OfflineMode)
            //{
            //    return;
            //}


            bool doDispatch = true;
            while (PhotonNetwork.IsMessageQueueRunning && doDispatch)
            {
                // DispatchIncomingCommands() returns true of it found any command to dispatch (event, result or state change)
                Profiler.BeginSample("DispatchIncomingCommands");
                doDispatch = PhotonNetwork.NetworkingClient.LoadBalancingPeer.DispatchIncomingCommands();
                Profiler.EndSample();
            }
        }


        public void OnCreatedRoom()
        {
            PhotonNetwork.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName);
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            PhotonNetwork.LoadLevelIfSynced();
        }


        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            var views = PhotonNetwork.PhotonViewCollection;
            foreach (var view in views)
            {
                view.RebuildControllerCache();
            }
        }

        public void OnFriendListUpdate(System.Collections.Generic.List<FriendInfo> friendList) { }

        public void OnCreateRoomFailed(short returnCode, string message) { }

        public void OnJoinRoomFailed(short returnCode, string message) { }

        public void OnJoinRandomFailed(short returnCode, string message) { }

        protected List<int> reusableIntList = new List<int>();

        public void OnJoinedRoom()
        {

            if (PhotonNetwork.ViewCount == 0)
                return;

            var views = PhotonNetwork.PhotonViewCollection;

            bool amMasterClient = PhotonNetwork.IsMasterClient;
            bool amRejoiningMaster = amMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1;

            if (amRejoiningMaster)
                reusableIntList.Clear();

            // If this is the master rejoining, reassert ownership of non-creator owners
            foreach (var view in views)
            {
                int viewOwnerId = view.OwnerActorNr;
                int viewCreatorId = view.CreatorActorNr;

                // Scene objects need to set their controller to the master.
                if (PhotonNetwork.IsMasterClient)
                    view.RebuildControllerCache();

                // Rejoining master should enforce its world view, and override any changes that happened while it was soft disconnected
                if (amRejoiningMaster)
                    if (viewOwnerId != viewCreatorId)
                    {
                        reusableIntList.Add(view.ViewID);
                        reusableIntList.Add(viewOwnerId);
                    }
            }

            if (amRejoiningMaster && reusableIntList.Count > 0)
            {
                PhotonNetwork.OwnershipUpdate(reusableIntList.ToArray());
            }
        }

        public void OnLeftRoom()
        {
            // Destroy spawned objects and reset scene objects
            PhotonNetwork.LocalCleanupAnythingInstantiated(true);
        }


        public void OnPlayerEnteredRoom(Player newPlayer)
        {

            bool isRejoiningMaster = newPlayer.IsMasterClient;
            bool amMasterClient = PhotonNetwork.IsMasterClient;

            // Nothing to do if this isn't the master joining, nor are we the master.
            if (!isRejoiningMaster && !amMasterClient)
                return;

            var views = PhotonNetwork.PhotonViewCollection;

            // Get a slice big enough for worst case - all views with no compression...extra byte per int for varint bloat.

            if (amMasterClient)
                reusableIntList.Clear();

            foreach (var view in views)
            {
                // TODO: make this only if the new actor affects this?
                view.RebuildControllerCache();

                //// If this is the master, and some other player joined - notify them of any non-creator ownership
                if (amMasterClient)
                {
                    int viewOwnerId = view.OwnerActorNr;
                    // TODO: Ideally all of this would only be targetted at the new player.
                    if (viewOwnerId != view.CreatorActorNr)
                    {
                        reusableIntList.Add(view.ViewID);
                        reusableIntList.Add(viewOwnerId);
                        //PhotonNetwork.TransferOwnership(view.ViewID, viewOwnerId);
                    }
                }
                // Master rejoined - reset all ownership. The master will be broadcasting non-creator ownership shortly
                else if (isRejoiningMaster)
                {
                    view.ResetOwnership();
                }
            }

            if (amMasterClient)
            {
                PhotonNetwork.OwnershipUpdate(reusableIntList.ToArray(), newPlayer.ActorNumber);
            }

        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            var views = PhotonNetwork.PhotonViewCollection;

            int leavingPlayerId = otherPlayer.ActorNumber;
            bool isInactive = otherPlayer.IsInactive;

            // SOFT DISCONNECT: A player has timed out to the relay but has not yet exceeded PlayerTTL and may reconnect.
            // Master will take control of this objects until the player hard disconnects, or returns.
            if (isInactive)
            {
                foreach (var view in views)
                {
                    if (view.OwnerActorNr == leavingPlayerId)
                        view.RebuildControllerCache(true);
                }

            }
            // HARD DISCONNECT: Player permanently removed. Remove that actor as owner for all items they created (Unless AutoCleanUp is false)
            else
            {
                bool autocleanup = PhotonNetwork.CurrentRoom.AutoCleanUp;

                foreach (var view in views)
                {
                    // Skip changing Owner/Controller for items that will be cleaned up.
                    if (autocleanup && view.CreatorActorNr == leavingPlayerId)
                        continue;

                    // Any views owned by the leaving player, default to null owner (which will become master controlled).
                    if (view.OwnerActorNr == leavingPlayerId || view.ControllerActorNr == leavingPlayerId)
                    {
                        view.SetOwnerInternal(null, 0);
                    }
                }
            }

        }
    }
}