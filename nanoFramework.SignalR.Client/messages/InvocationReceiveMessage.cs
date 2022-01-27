using System;
using System.Collections;
using System.Text;

namespace nanoFramework.SignalR.Client
{
    internal class InvocationReceiveMessage
    {
        public MessageType type { get; set; }
        public Hashtable headers { get; set; }
        public string invocationId { get; set; }
        public string target { get; set; }
        public ArrayList arguments { get; set; }
        public string[] streamIds { get; set; }
        public string error { get; set; }
        public bool allowReconnect { get; set; }
        public object result { get; set; }
    }
}
