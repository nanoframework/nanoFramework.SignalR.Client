// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.WebSockets;

namespace nanoFramework.SignalR.Client
{
    /// <summary>
    /// Options to use with a <see cref="HubConnection"/> object.
    /// </summary>
    public class HubConnectionOptions : ClientWebSocketOptions
    {
        /// <summary>
        /// Set to true to enable automatic reconnect to the server.
        /// </summary>
        /// <remarks>
        /// Client will only reconnect if this is indicated by the server close message. 
        /// This reconnect function is experimental and perhaps better handled elsewhere. 
        /// </remarks>
        public bool Reconnect { get; set; } = false;
    }
}
