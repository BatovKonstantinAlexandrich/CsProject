using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Threading;
using System.Threading.Tasks;

public class MessageClient : AsyncContext, IHostedService
{    
    private string                  Url;
    private IHubConnectionBuilder   Builder;
    private HubConnection           Connection;
    public MessageClient(string Url="https://localhost:5001/DevHub")
    {
        this.Url = Url;
        Builder = new HubConnectionBuilder().WithUrl(this.Url, Configure);
    }
    public void Configure(HttpConnectionOptions options)
    {        
        options.SkipNegotiation = true;
        options.Transports = HttpTransportType.WebSockets;
    }


    [Label("")]
    public async Task Connect()
    {
        Console.WriteLine("Connecting to URL: "+Url);
        Connection = Builder.Build();
        Connection.On<string>("Response", OnResponse);
        Connection.Closed += async (Exception exception) =>
        {
            Console.WriteLine(exception);
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await Connection.StartAsync();
        };
        await Connection.StartAsync();        
        Console.WriteLine("Connected to URL: " + Url);
    }




    private void OnResponse(string ResponseText)
    {
        Console.WriteLine("Handling Response: \n" + ResponseText);
        DataResponseMessage ResponseMessage = JsonConvert.DeserializeObject<DataResponseMessage>(ResponseText);
        var Todo = Take(ResponseMessage.SerialKey);
        if (ResponseMessage.ActionStatus == "Success")
        {
            Console.WriteLine("Handled Response: \n" + ResponseText);
            Todo(ResponseMessage.MessageObject);
        }
        else
        {
            Console.WriteLine("Операция не выполнена: "+ ResponseMessage.SerialKey);
        }        
    }


    public async Task Request(object Message, Action<object> Handle)
    {
        Console.WriteLine("Requesting: \n"+ JObject.FromObject(Message).ToString());
        string SerialKey = Put(Handle);
        var RequestMessage = new DataRequestMessage() {
            SerialKey = SerialKey,
            //MessageObject = Message,

            //TODO:
            ActionName = "",

            //TODO:
            AccessToken = ""
        };
        string RequestText = JObject.FromObject(RequestMessage).ToString();        
        await Connection.InvokeAsync("Request", RequestText);
        Console.WriteLine("Requested: \n" + RequestText);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Connect();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (Connection != null)
        {
            return Connection.StopAsync(cancellationToken);
        }

        return Task.CompletedTask;
    }
}
