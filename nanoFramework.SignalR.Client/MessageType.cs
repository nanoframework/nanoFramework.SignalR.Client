using System;
using System.Text;

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
