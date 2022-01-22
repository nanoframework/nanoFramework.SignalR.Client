using System;
using System.Text;

namespace nanoFramework.SignalR.Client
{
    /// <summary>
    /// Describes the current state of the <see cref="HubConnection"/> to the server.
    /// </summary>
    public enum HubConnectionState
    {
        /// <summary>
        /// The hub connection is disconnected.
        /// </summary>
        Disconnected = 0,
        /// <summary>
        /// The hub connection is connected.
        /// </summary>
        Connected = 1,
        /// <summary>
        /// The hub connection is connecting.
        /// </summary>
        Connecting = 2,
        /// <summary>
        /// The hub connection is reconnecting.
        /// </summary>
        Reconnecting = 3
    }
}
