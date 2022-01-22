using System;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using nanoFramework.Networking;
using nanoFramework.SignalR.Client;
//using nanoFramework.SignalR.Client.Json;


namespace NFSignalrTestApp
{
    public class Program
    {
        public static void Main()
        {


            

            Debug.WriteLine("Hello from nanoFramework!");


            const string Ssid = "testnetwork";
            const string Password = "securepassword1!";
            // Give 60 seconds to the wifi join to happen
            CancellationTokenSource cs = new(60000);
            var success = WiFiNetworkHelper.ScanAndConnectDhcp(Ssid, Password, token: cs.Token);
            if (!success)
            {
                // Something went wrong, you can get details with the ConnectionError property:
                Debug.WriteLine($"Can't connect to the network, error: ");
                //if (NetworkHelper.ConnectionError.Exception != null)
                //{
                //    Debug.WriteLine($"ex: { NetworkHelper.ConnectionError.Exception}");
                //}
            }

            Debug.WriteLine(IPAddress.GetDefaultLocalAddress().ToString());


            var headers = new ClientWebSocketHeaders();
            headers["test"] = "testin123";

            HubConnection hubConnection = new HubConnection("ws://192.168.179.2:5001/chathub");
            hubConnection.Closed += HubConnection_Closed;
            hubConnection.On("ReceiveMessage", new Type[] { typeof(string), typeof(string) }, OnReceivedMessage);
            hubConnection.Start();



            if (hubConnection.State == HubConnectionState.Connected)
            {
                try
                {
                    int result = ((int)hubConnection.InvokeCore("Add", typeof(int), new object[] { 1, 2 }));
                    Debug.WriteLine(result.ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            Thread.Sleep(1000);


            var x = hubConnection.InvokeCore("Do", null, new object[] { 1, 2 });

            var y = hubConnection.InvokeCore("Nothing", null, new object[] { });
            int number = (int)hubConnection.InvokeCoreAsync("Add", typeof(int), new object[] { 1, 3 }).Value;
            Debug.WriteLine(number.ToString());

            Thread.Sleep(7000);
            var asynObject = hubConnection.InvokeCoreAsync("Add", typeof(int), new object[] { 1, 3 });
            while (!asynObject.Completed)
            {
                Debug.WriteLine("*");
                Thread.Sleep(500);
            }
            Debug.WriteLine(((int)asynObject.Value).ToString());


            while (true)
            {
                hubConnection.SendCore("SendMessage", new object[] { "Feiko", "testing123" });
                Thread.Sleep(2000);
            }



            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }

        private static void HubConnection_Closed(object sender, SignalrEventMessageArgs message)
        {
            Debug.WriteLine($"closed received with message: {message.Message}");
        }

        public static void OnReceivedMessage(object sender, object[] args)
        {
            var name = args[0] as string;
            var message = args[1] as string;

            Console.WriteLine($"{name} : {message}");
        }
    }
}
