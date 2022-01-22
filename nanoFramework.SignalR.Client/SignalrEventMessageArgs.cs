using System;
using System.Text;

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
