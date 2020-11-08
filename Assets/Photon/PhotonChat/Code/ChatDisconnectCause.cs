// ----------------------------------------------------------------------------------------------------------------------
// <summary>The Photon Chat Api enables clients to connect to a chat server and communicate with other clients.</summary>
// <remarks>ChatClient is the main class of this api.</remarks>
// <copyright company="Exit Games GmbH">Photon Chat Api - Copyright (C) 2014 Exit Games GmbH</copyright>
// ----------------------------------------------------------------------------------------------------------------------

namespace Photon.Chat
{
    /// <summary>Enumeration of causes for Disconnects (used in <see cref="ChatClient.DisconnectedCause"/>).</summary>
    /// <remarks>Read the individual descriptions to find out what to do about this type of disconnect.</remarks>
    public enum ChatDisconnectCause
    {
        /// <summary>No error was tracked.</summary>
        None,
        /// <summary>OnStatusChanged: The server is not available or the address is wrong. Make sure the port is provided and the server is up.</summary>
        ExceptionOnConnect,
        /// <summary>OnStatusChanged: The server disconnected this client from within the room's logic (the C# code).</summary>
        DisconnectByServerLogic,
        /// <summary>OnStatusChanged: The server disconnected this client for unknown reasons.</summary>
        DisconnectByServerReasonUnknown,
        /// <summary>OnStatusChanged: The server disconnected this client due to timing out (missing acknowledgement from the client).</summary>
        ServerTimeout,

        /// <summary>OnStatusChanged: This client detected that the server's responses are not received in due time.</summary>
        ClientTimeout,
        /// <summary>OnStatusChanged: Some internal exception caused the socket code to fail. Contact Exit Games.</summary>
        Exception,
        /// <summary>OnOperationResponse: Authenticate in the Photon Cloud with invalid AppId. Update your subscription or contact Exit Games.</summary>
        InvalidAuthentication,
        /// <summary>OnOperationResponse: Authenticate (temporarily) failed when using a Photon Cloud subscription without CCU Burst. Update your subscription.</summary>
        MaxCcuReached,
        /// <summary>OnOperationResponse: Authenticate when the app's Photon Cloud subscription is locked to some (other) region(s). Update your subscription or change region.</summary>
        InvalidRegion,
        /// <summary>OnOperationResponse: Operation that's (currently) not available for this client (not authorized usually). Only tracked for op Authenticate.</summary>
        OperationNotAllowedInCurrentState,
        /// <summary>OnOperationResponse: Authenticate in the Photon Cloud with invalid client values or custom authentication setup in Cloud Dashboard.</summary>
        CustomAuthenticationFailed,
        /// <summary>The authentication ticket should provide access to any Photon Cloud server without doing another authentication-service call. However, the ticket expired.</summary>
        AuthenticationTicketExpired,
        /// <summary>OnStatusChanged: The client disconnected from within the logic (the C# code).</summary>
        DisconnectByClientLogic
    }
}