// -----------------------------------------------------------------------
// <copyright file="ChatAppSettings.cs" company="Exit Games GmbH">
//   Chat API for Photon - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>Settings for Photon Chat application and the server to connect to.</summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif


namespace Photon.Chat
{
    using System;
    using ExitGames.Client.Photon;
    
    /// <summary>
    /// Settings for Photon application(s) and the server to connect to.
    /// </summary>
    /// <remarks>
    /// This is Serializable for Unity, so it can be included in ScriptableObject instances.
    /// </remarks>
    #if !NETFX_CORE || SUPPORTED_UNITY
    [Serializable]
    #endif
    public class ChatAppSettings
    {
        /// <summary>AppId for the Chat Api.</summary>
        public string AppId;
        
        /// <summary>The AppVersion can be used to identify builds and will split the AppId distinct "Virtual AppIds" (important for the users to find each other).</summary>
        public string AppVersion;
        
        /// <summary>Can be set to any of the Photon Cloud's region names to directly connect to that region.</summary>
        public string FixedRegion;
        
        /// <summary>The address (hostname or IP) of the server to connect to.</summary>
        public string Server;
        
        /// <summary>The network level protocol to use.</summary>
        public ConnectionProtocol Protocol = ConnectionProtocol.Udp;

        /// <summary>Log level for the network lib.</summary>
        public DebugLevel NetworkLogging = DebugLevel.ERROR;

        /// <summary>If true, the default nameserver address for the Photon Cloud should be used.</summary>
        public bool IsDefaultNameServer { get { return string.IsNullOrEmpty(this.Server); } }
    }
}