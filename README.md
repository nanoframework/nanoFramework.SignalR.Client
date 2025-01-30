[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_nanoFramework.SignalR.Client&metric=alert_status)](https://sonarcloud.io/dashboard?id=nanoframework_nanoFramework.SignalR.Client) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_nanoFramework.SignalR.Client&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=nanoframework_nanoFramework.SignalR.Client) [![NuGet](https://img.shields.io/nuget/dt/nanoFramework.SignalR.Client.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.SignalR.Client/) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/nanoframework/Home/blob/main/CONTRIBUTING.md) [![Discord](https://img.shields.io/discord/478725473862549535.svg?logo=discord&logoColor=white&label=Discord&color=7289DA)](https://discord.gg/gCyBu8T)

![nanoFramework logo](https://raw.githubusercontent.com/nanoframework/Home/main/resources/logo/nanoFramework-repo-logo.png)

-----
document language: [English](README.md) | [简体中文](README.zh-cn.md)

# Welcome to the .NET **nanoFramework** SignalR Client Library repository

This API mirrors (as close as possible) the official .NET [Microsoft.AspNetCore.SignalR.Client](https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client). Exceptions are mainly derived from the lack of `async` and generics support in .NET **nanoFramework**.

## Build status

| Component | Build Status | NuGet Package |
|:-|---|---|
| nanoFramework.SignalR.Client | [![Build Status](https://dev.azure.com/nanoframework/nanoFramework.SignalR.Client/_apis/build/status/nanoFramework.SignalR.Client?repoName=nanoframework%2FnanoFramework.SignalR.Client&branchName=main)](https://dev.azure.com/nanoframework/nanoFramework.SignalR.Client/_build/latest?definitionId=91&repoName=nanoframework%2FnanoFramework.SignalR.Client&branchName=main) | [![NuGet](https://img.shields.io/nuget/v/nanoFramework.SignalR.Client.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.SignalR.Client/) |

# Usage

This is a SignalR Client library that enable you to connect your .net nanoFramework device to a SignalR Hub.  SignalR is part of the ASP.NET Framework that makes it easy to create web applications that require high-frequency updates from the server like gaming. In the IoT domain SignalR can be used to create a webapp that for example shows a life graphs of connected smart meters, control a robot arm and many more.

Important: You must be connected to a network with a valid IP address. Please check the examples with the Network Helpers on how to set this up.

### Connect to a hub

To establish a connection, create a `HubConnection` Client. You have to set the hub URL upon initialization of the HubConnection. You can also set custom headers by adding `ClientWebsocketHeaders` and set extra options by adding `HubConnectionOptions` upon initialization. The options are mainly used to change the settings of the underlying websocket and to set extra ssl options.
You can start the connection by calling `Start`.

```csharp
using System;
using System.Diagnostics;
using System.Threading;
using nanoFramework.SignalR.Client;

namespace NFSignalrTestClient
{
    public class Program
    {
        public static void Main()
        {
            //setup connection
            var options = new HubConnectionOptions() { Reconnect = true };
            HubConnection hubConnection = new HubConnection("http://YourSignalrTestServer/testhub", options: options);
            
            hubConnection.Closed += HubConnection_Closed;

            hubConnection.On("ReceiveMessage", new Type[] { typeof(string), typeof(string) }, (sender, args) =>
            {
                var name = (string)args[0];
                var message = (string)args[1];

                Console.WriteLine($"{name} : {message}");
            });
            
            //start connection
            hubConnection.Start();
                     

            AsyncResult dashboardClientConnected = hubConnection.InvokeCoreAsync("AwaitCientConnected", typeof(bool), new object[] { }, -1);

            int seconds = 0;

            while (!dashboardClientConnected.Completed)
            {
                Debug.WriteLine($"Waited {seconds} for client to open webapp");
                seconds++;
                Thread.Sleep(1000);
            }

            if ((bool)dashboardClientConnected.Value)
            {
                hubConnection.SendCore("ReportStatus", new object[] { "Client Connected" });

                int count = 0;
                while (hubConnection.State == HubConnectionState.Connected)
                {
                    hubConnection.InvokeCore("SendMessage", null, new object[] { count, "this is a control message" });
                    count++;
                    Thread.Sleep(1000);
                }
            }
            else
            {
                hubConnection.Stop("client failed to connect");
            }
        }

        private static void HubConnection_Closed(object sender, SignalrEventMessageArgs message)
        {
            Debug.WriteLine($"closed received with message: {message.Message}");
        }
    }
}
```

##### Handle lost connections

Reconnecting
By default the `HubConnection` Client will not reconnect if a connection is lost or fails upon first connection. By setting the HubConnectionOptions `Reconnect` to true upon initialization of the HubConnection, the client will try to reconnect with a interval of 0, 2, 10, and 30 seconds, stopping after four failed attempts. 
When the client tries to reconnect the Reconnecting event is fired.

> Note: the client will only try to reconnect if the connection is closed after receiving a server close message with the server request to reconnect. 

##### Connection State

The connection state can be monitored by checking the HubConnection `State`. The different states are: `Disconnected` `Connected` `Connecting` and `Reconnecting`. After a connection is established every state change will fire a event: `Closed` `Reconnecting` or `Reconnected`. These events can be used to manual handle disconnects and reconnection.   
 
### Call Hub methods from Client

There are three ways to call a method on the hub. All three methods require you to pass the hub method name and any arguments defined in the hub method. 

The simples form is calling `SendCore` This will call the method without expecting or waiting for any response from the server. It’s a ‘fire and forget’ method. 

The second method is `InvokeCore`. InvokeCore requires you to pass the hub method name, the hub method return type, the arguments and an optional timeout in milliseconds. If no timeout is given the default `ServerTimeout` is used. This is a Synchronous method that will wait until the server replies. The returned object is of the type defined by the method return type argument. The casting of the object to the right type should be done manually.
Note: if the hub method return type is void, the return type upon calling InvokeCore or InvokeCoreAsync should be null.

Note: set timeout to -1 to disable the server timeout. If no return message is received from the server this will wait indefinitely.   

The third method is `InvokeCoreAsync`. This is the same as InvokeCore but than asynchronous. It will return an AsyncResult. 

##### AsyncResult

The `AsyncResult` monitors the return message of the hub method. Upon completion `Completed` will be true. Upon completion the `Value` will hold the return object that needs to be cast to the right Type manually. Calling `Value` before completion will result in the awaiting of the server return. If an error occurs, `Error` will be true and the error message will be inside `ErrorMessage`.

```csharp
AsyncResult dashboardClientConnected = hubConnection.InvokeCoreAsync("AwaitCientConnected", typeof(bool), new object[] { }, -1);

int seconds = 0;

while (!dashboardClientConnected.Completed)
{
    Debug.WriteLine($"Waited {seconds} for client to open webapp");
    seconds++;
    Thread.Sleep(1000);
}

if ((bool)dashboardClientConnected.Value)
{
    Debug.WriteLine("The client connected to the dashboard, start sending live data");
}
```

### Call clients methods from hub

Define methods the hub calls using connection.On after building, but before starting the connection.

```csharp
connection.On("ReceiveMessage", new Type[] { typeof(string), typeof(string) }, (sender, args) =>
{
    var name = args[0] as string;
    var message = args[1] as string;

    Debug.WriteLine($"{name} : {message}");
});
```

The preceding code in connection.On runs when server-side code calls it using the SendAsync method.

```csharp
public async Task SendMessage(string user, string message)
{
    await Clients.All.SendAsync("ReceiveMessage", user, message);
}
```

### Stopping the connection

The connection can be closed by calling `Stop`. This will stop the connection. If you want to stop the connection because for example a device sensor malfunctions, an error can be conveyed back to the server by stating the error using the optional `errorMessage`.  

## Feedback and documentation

For documentation, providing feedback, issues and finding out how to contribute please refer to the [Home repo](https://github.com/nanoframework/Home).

Join our Discord community [here](https://discord.gg/gCyBu8T).

## Credits

The list of contributors to this project can be found at [CONTRIBUTORS](https://github.com/nanoframework/Home/blob/main/CONTRIBUTORS.md).

## License

The **nanoFramework** Class Libraries are licensed under the [MIT license](LICENSE.md).

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behaviour in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

## .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).
