using System;
using System.Text;

namespace nanoFramework.SignalR.Client
{
    public class InvocationBlockingSendMessage : InvocationSendMessage
    {
        public string invocationId { get; set; }
    }
}
