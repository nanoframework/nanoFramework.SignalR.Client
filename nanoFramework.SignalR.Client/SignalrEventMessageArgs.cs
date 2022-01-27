// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace nanoFramework.SignalR.Client
{
    /// <summary>
    /// The event arguments that are used when <see cref="HubConnection.SignalrEvent"/> is called
    /// </summary>
    public class SignalrEventMessageArgs : EventArgs
    {
        /// <summary>
        /// The optional message from the server for when a connection change occurs. 
        /// </summary>
        public string Message { get; set; }
    }
}
