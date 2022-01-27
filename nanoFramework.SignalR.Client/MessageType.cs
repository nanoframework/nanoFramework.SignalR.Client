// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace nanoFramework.SignalR.Client
{
    internal enum MessageType
    {
        Invocation = 1,
        StreamInvocation = 4,
        StreamItem = 2,
        Completion = 3,
        CancelInvocation = 5,
        Ping = 6,
        Close = 7
    }
}
