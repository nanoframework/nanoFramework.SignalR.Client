using System;
using System.Collections;
using System.Text;

namespace nanoFramework.SignalR.Client
{
    internal class InvocationSendMessage
    {
        public int type { get; set; }
        public string target { get; set; }
        public ArrayList arguments { get; set; }
    }
}
