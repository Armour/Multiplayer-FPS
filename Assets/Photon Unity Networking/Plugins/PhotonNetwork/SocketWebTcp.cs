// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocketTcp.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Internal class to encapsulate the network i/o functionality for the realtime libary.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using UnityEngine;

#if UNITY_WEBGL

namespace ExitGames.Client.Photon
{
    /// <summary>
    /// Internal class to encapsulate the network i/o functionality for the realtime libary.
    /// </summary>
    internal class SocketWebTcp : IPhotonSocket, IDisposable
    {
        private WebSocket sock;

        private readonly object syncer = new object();

        public SocketWebTcp(PeerBase npeer) : base(npeer)
        {
            ServerAddress = npeer.ServerAddress;
            if (this.ReportDebugOfLevel(DebugLevel.INFO))
            {
                Listener.DebugReturn(DebugLevel.INFO, "new SocketWebTcp() " + ServerAddress);
            }

            Protocol = ConnectionProtocol.Tcp;
            PollReceive = false;
        }

        public void Dispose()
        {
            this.State = PhotonSocketState.Disconnecting;

            if (this.sock != null)
            {
                try
                {
                    if (this.sock.Connected) this.sock.Close();
                }
                catch (Exception ex)
                {
                    this.EnqueueDebugReturn(DebugLevel.INFO, "Exception in Dispose(): " + ex);
                }
            }

            this.sock = null;
            this.State = PhotonSocketState.Disconnected;
        }

        GameObject websocketConnectionObject;
        public override bool Connect()
        {
            //bool baseOk = base.Connect();
            //if (!baseOk)
            //{
            //    return false;
            //}


            State = PhotonSocketState.Connecting;

            if (websocketConnectionObject != null)
            {
                UnityEngine.Object.Destroy(websocketConnectionObject);
            }

            websocketConnectionObject = new GameObject("websocketConnectionObject");
            MonoBehaviour mb = websocketConnectionObject.AddComponent<MonoBehaviour>();
            // TODO: not hidden for debug
            //websocketConnectionObject.hideFlags = HideFlags.HideInHierarchy;
            UnityEngine.Object.DontDestroyOnLoad(websocketConnectionObject);

            this.sock = new WebSocket(new Uri(ServerAddress));
            mb.StartCoroutine(this.sock.Connect());

            mb.StartCoroutine(ReceiveLoop());
            return true;
        }


        public override bool Disconnect()
        {
            if (ReportDebugOfLevel(DebugLevel.INFO))
            {
                this.Listener.DebugReturn(DebugLevel.INFO, "SocketTcp.Disconnect()");
            }

            State = PhotonSocketState.Disconnecting;

            lock (this.syncer)
            {
                if (this.sock != null)
                {
                    try
                    {
                        this.sock.Close();
                    }
                    catch (Exception ex)
                    {
                        this.Listener.DebugReturn(DebugLevel.ERROR, "Exception in Disconnect(): " + ex);
                    }
                    this.sock = null;
                }
            }

            if (websocketConnectionObject != null)
            {
                UnityEngine.Object.Destroy(websocketConnectionObject);
            }

            State = PhotonSocketState.Disconnected;
            return true;
        }

        /// <summary>
        /// used by TPeer*
        /// </summary>
        public override PhotonSocketError Send(byte[] data, int length)
        {
            if (this.State != PhotonSocketState.Connected)
            {
                return PhotonSocketError.Skipped;
            }

            try
            {
                if (this.ReportDebugOfLevel(DebugLevel.ALL))
                {
                    this.Listener.DebugReturn(DebugLevel.ALL, "Sending: " + SupportClass.ByteArrayToString(data));
                }
                this.sock.Send(data);
            }
            catch (Exception e)
            {
                this.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send to: " + this.ServerAddress + ". " + e.Message);

                HandleException(StatusCode.Exception);
                return PhotonSocketError.Exception;
            }

            return PhotonSocketError.Success;
        }

        public override PhotonSocketError Receive(out byte[] data)
        {
            data = null;
            return PhotonSocketError.NoData;
        }


        internal const int ALL_HEADER_BYTES = 9;
        internal const int TCP_HEADER_BYTES = 7;
        internal const int MSG_HEADER_BYTES = 2;

        public IEnumerator ReceiveLoop()
        {
            this.Listener.DebugReturn(DebugLevel.INFO, "ReceiveLoop()");
            while (!this.sock.Connected && this.sock.Error == null)
            {
                yield return new WaitForSeconds(0.1f);
            }
            if (this.sock.Error != null)
            {
                this.Listener.DebugReturn(DebugLevel.ERROR, "Exiting receive thread due to error: " + this.sock.Error + " Server: " + this.ServerAddress);
				this.HandleException(StatusCode.ExceptionOnConnect);
            }
            else
            {
                if (this.ReportDebugOfLevel(DebugLevel.ALL))
                {
                    this.Listener.DebugReturn(DebugLevel.ALL, "Receiving by websocket. this.State: " + State);
                }
                State = PhotonSocketState.Connected;
				while (State == PhotonSocketState.Connected)
				{
					if (this.sock.Error != null)
					{
						this.Listener.DebugReturn(DebugLevel.ERROR, "Exiting receive thread (inside loop) due to error: " + this.sock.Error + " Server: " + this.ServerAddress);
						this.HandleException(StatusCode.ExceptionOnReceive);
						break;
					}
					else
					{
						byte[] inBuff = this.sock.Recv();
						if (inBuff == null || inBuff.Length == 0)
						{
							yield return new WaitForSeconds(0.1f);
							continue;
						}

						if (this.ReportDebugOfLevel(DebugLevel.ALL))
						{
							this.Listener.DebugReturn(DebugLevel.ALL, "TCP << " + inBuff.Length + " = " + SupportClass.ByteArrayToString(inBuff));
						}


						// check if it's a ping-result (first byte = 0xF0). this is 9 bytes in total. no other headers!
						// note: its a coincidence that ping-result-size == header-size. if this changes we have to refactor this
						if (inBuff[0] == 0xF0)
						{
							try
							{
								HandleReceivedDatagram(inBuff, inBuff.Length, false);
							}
							catch (Exception e)
							{
								if (this.ReportDebugOfLevel(DebugLevel.ERROR))
								{
									this.EnqueueDebugReturn(DebugLevel.ERROR, "Receive issue. State: " + this.State + ". Server: '" + this.ServerAddress + "' Exception: " + e);
								}
								this.HandleException(StatusCode.ExceptionOnReceive);
							}
							continue;
						}

						// get data and split the datagram into two buffers: head and body
						if (inBuff.Length > 0)
						{
							try
							{
								HandleReceivedDatagram(inBuff, inBuff.Length, false);
							}
							catch (Exception e)
							{
								if (this.ReportDebugOfLevel(DebugLevel.ERROR))
								{
									this.EnqueueDebugReturn(DebugLevel.ERROR, "Receive issue. State: " + this.State + ". Server: '" + this.ServerAddress + "' Exception: " + e);
								}
								this.HandleException(StatusCode.ExceptionOnReceive);
							}
						}
					}
				}
            }

            Disconnect();
        }
    }
}

#endif