[![质量检验关状态](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_nanoFramework.SignalR.Client&metric=alert_status)](https://sonarcloud.io/dashboard?id=nanoframework_nanoFramework.SignalR.Client) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_nanoFramework.SignalR.Client&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=nanoframework_nanoFramework.SignalR.Client) [![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE) [![NuGet](https://img.shields.io/nuget/dt/nanoFramework.SignalR.Client.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.SignalR.Client/) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/nanoframework/Home/blob/main/CONTRIBUTING.md) [![Discord](https://img.shields.io/discord/478725473862549535.svg?logo=discord&logoColor=white&label=Discord&color=7289DA)](https://discord.gg/gCyBu8T)

![nanoFramework logo](https://raw.githubusercontent.com/nanoframework/Home/main/resources/logo/nanoFramework-repo-logo.png)

-----
文档语言: [English](README.md) | [简体中文](README.zh-cn.md)

# 欢迎使用。net **nanoFramework** SignalR客户端库

这个API镜像(尽可能接近)官方的.net [Microsoft.AspNetCore.SignalR.Client](https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client)。异常主要源于.net **纳米框架**中缺乏“异步”和泛型支持。

## 构建状态

| Component | 构建状态 | NuGet Package |
|:-|---|---|
| nanoFramework.SignalR.Client | [![Build Status](https://dev.azure.com/nanoframework/nanoFramework.SignalR.Client/_apis/build/status/nanoFramework.SignalR.Client?repoName=nanoframework%2FnanoFramework.SignalR.Client&branchName=main)](https://dev.azure.com/nanoframework/nanoFramework.SignalR.Client/_build/latest?definitionId=91&repoName=nanoframework%2FnanoFramework.SignalR.Client&branchName=main) | [![NuGet](https://img.shields.io/nuget/v/nanoFramework.SignalR.Client.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.SignalR.Client/) |

# 使用

这是一个SignalR客户端库，使您能够将。net纳米框架设备连接到SignalR集线器。SignalR是ASP的一部分。NET框架，它可以轻松地创建需要从服务器进行高频更新的web应用程序，比如游戏。在物联网领域，SignalR可用于创建web应用程序，例如显示连接的智能电表的生命图，控制机械臂等。

重要提示:您必须连接到具有有效IP地址的网络。请查看网络助手的示例，了解如何设置它。

## 连接到hub

要建立连接，请创建“HubConnection”客户端。您必须在初始化HubConnection时设置集线器URL。您还可以通过添加' ClientWebsocketHeaders '来设置自定义头信息，并通过在初始化时添加' HubConnectionOptions '来设置额外的选项。这些选项主要用于更改底层websocket的设置和设置额外的ssl选项。

您可以通过调用“start”启动连接。

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
            //设置连接
            var options = new HubConnectionOptions() { Reconnect = true };
            HubConnection hubConnection = new HubConnection("http://YourSignalrTestServer/testhub", options: options);
            
            hubConnection.Closed += HubConnection_Closed;

            hubConnection.On("ReceiveMessage", new Type[] { typeof(string), typeof(string) }, (sender, args) =>
            {
                var name = (string)args[0];
                var message = (string)args[1];

                Console.WriteLine($"{name} : {message}");
            });
            
            //开始连接
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
                hubConnection.SendCore("ReportStatus", new object[] { "客户端连接" });

                int count = 0;
                while (hubConnection.State == HubConnectionState.Connected)
                {
                    hubConnection.InvokeCore("SendMessage", null, new object[] { count, "这是一条控制信息" });
                    count++;
                    Thread.Sleep(1000);
                }
            }
            else
            {
                hubConnection.Stop("客户端连接失败");
            }
        }

        private static void HubConnection_Closed(object sender, SignalrEventMessageArgs message)
        {
            Debug.WriteLine($"已收到消息:{message.Message}");
        }
    }
}
```

##### 处理失去连接

重新连接

默认情况下，如果连接丢失或第一次连接失败，' HubConnection '客户端将不会重新连接。通过在HubConnection初始化时将HubConnectionOptions ' Reconnect '设置为true，客户端将尝试以0、2、10和30秒的间隔重新连接，在四次失败尝试后停止。
当客户端尝试重新连接时，将触发重新连接事件。

> Note: the client will only try to reconnect if the connection is closed after receiving a server close message with the server request to reconnect.

##### 连接状态

可以通过检查HubConnection ' state '来监视连接状态。不同的状态是:“断开”、“连接”、“连接”和“重新连接”。连接建立后，每次状态变化都会触发一个事件:'关闭' '重新连接'或'重新连接'。这些事件可用于手动处理断开和重新连接。

### 从客户端调用Hub方法

在集线器上调用方法有三种方法。这三个方法都要求您传递集线器方法名称和在集线器方法中定义的任何参数。

这将调用该方法，而不期望或等待来自服务器的任何响应。这是一种“发了就忘了”的方法。

第二种方法是' InvokeCore '。InvokeCore要求您传递集线器方法名称、集线器方法返回类型、参数和一个以毫秒为单位的可选超时。如果没有给出超时，则使用默认的' ServerTimeout '。这是一个同步方法，它将等待服务器的响应。返回的对象的类型由方法return type参数定义。应该手动将对象转换为正确的类型。

Note: 如果hub方法的返回类型是void，那么调用InvokeCore或InvokeCoreAsync时的返回类型应该是null。

Note: 将timeout设置为-1，以防止服务器超时。如果没有从服务器收到返回消息，这将无限期地等待。

第三个方法是' InvokeCoreAsync '。这与InvokeCore相同，但不是异步的。它将返回一个AsyncResult。

##### AsyncResult

' AsyncResult '监视hub方法的返回消息。完成后，“Completed”将为真。完成后，' Value '将保存需要手动转换为正确类型的返回对象。在完成之前调用' Value '将导致等待服务器返回。如果发生错误，' error '将为真，错误消息将在' ErrorMessage '中。

```csharp
AsyncResult dashboardClientConnected = hubConnection.InvokeCoreAsync("AwaitCientConnected", typeof(bool), new object[] { }, -1);

int seconds = 0;

while (!dashboardClientConnected.Completed)
{
    Debug.WriteLine($"等待{seconds}客户端打开webapp");
    seconds++;
    Thread.Sleep(1000);
}

if ((bool)dashboardClientConnected.Value)
{
    Debug.WriteLine("连接到仪表板的客户端，开始发送实时数据");
}
```

### 从集线器调用客户机方法

使用连接定义集线器调用的方法。在构建之后，但在开始连接之前。

```csharp
connection.On("ReceiveMessage", new Type[] { typeof(string), typeof(string) }, (sender, args) =>
{
    var name = args[0] as string;
    var message = args[1] as string;

    Debug.WriteLine($"{name} : {message}");
});
```

以上代码连接。On在服务器端代码使用SendAsync方法调用它时运行。

```csharp
public async Task SendMessage(string user, string message)
{
    await Clients.All.SendAsync("ReceiveMessage", user, message);
}
```

### 停止连接

通过调用“Stop”可以关闭连接。这将停止连接。如果您想停止连接，例如因为设备传感器故障，可以使用可选的' errorMessage '声明错误，将错误传递回服务器。

## 反馈和文档

关于文档，提供反馈，问题和找出如何贡献，请参考 [首页](https://github.com/nanoframework/Home).

加入我们的Discord社区[这里](https://discord.gg/gCyBu8T).

## Credits

这个项目的贡献者名单可以在 [贡献者](https://github.com/nanoframework/Home/blob/main/CONTRIBUTORS.md).

## License

**nanoFramework** 类库是根据 [MIT license](LICENSE.md).

## Code of Conduct

本项目采用了《贡献者盟约》所规定的行为准则，以澄清我们社区的预期行为。
有关更多信息，请参阅 [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

## .NET Foundation

这个项目是由 [.NET Foundation](https://dotnetfoundation.org) 支持.
