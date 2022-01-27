// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace nanoFramework.SignalR.Client
{
    internal class InvocationSendMessage
    {
        public MessageType type { get; set; }
        public string target { get; set; }
        public ArrayList arguments { get; set; }
    }
}
