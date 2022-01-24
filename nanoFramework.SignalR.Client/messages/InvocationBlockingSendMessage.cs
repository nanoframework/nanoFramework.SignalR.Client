using System;
using System.Text;

namespace nanoFramework.SignalR.Client
{
    internal class InvocationBlockingSendMessage : InvocationSendMessage
    {
        public string invocationId { get; set; }
    }
}
