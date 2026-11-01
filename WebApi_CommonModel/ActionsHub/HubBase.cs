using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Threading.Tasks;
/// <summary>
/// Обеспечивает выполнение операций RPC/
/// </summary>
/// 
public class HubBase: Hub
{
    protected readonly ILogger<ActionsHub> _logger;
    protected readonly IHttpContextAccessor _httpContextAccessor;

    protected readonly ConnectionsContext connectionsContext;
    protected readonly ActionsContext actionsContext;

    public HubBase(ILogger<ActionsHub> logger,
        IHttpContextAccessor httpContextAccessor,
        ConnectionsContext clientsContext,
        ActionsContext serviceContext)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        connectionsContext = clientsContext;
        actionsContext = serviceContext;
        
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"OnConnectedAsync({Context.ConnectionId})");
        var caller = Clients.Caller;
        var connectionId = Context.ConnectionId;
        var connectionContext = connectionsContext.AddClientConnection(Context.ConnectionId, (requestText) => {
            this._logger.LogInformation(connectionId);
            caller.SendAsync("Request", requestText).Wait();
        });        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation($"OnDisconnectedAsync({Context.ConnectionId},{(exception != null ? exception.Message : "")})");
        connectionsContext.RemoveClientConnection(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Выполнение запроса
    /// </summary>  
    public void SendRequest(string RequestText)
    {
        ActionRequest RequestMessage = DeserializeRequest(RequestText);
        Console.WriteLine("Requesting: \n" + RequestMessage.ActionName);

        var caller = Clients.Caller;
        var clientId = Context.ConnectionId;
        ExecuteRequest(RequestMessage, 
            resp => {

                this.connectionsContext.Connections[clientId].Request("Response",
                    new() {
                        { "ResponseText",new ActionResponse()
                        {
                            ActionStatus = "Success",
                            RequestMessage = RequestMessage,
                            SerialKey = RequestMessage.SerialKey,
                            MessageObject = resp
                        }.ToJson()}
                    }, r => { }, e => { }).Wait();
                 
            },
            err => {
                this.connectionsContext.Connections[clientId].Request("Response",
                    new() {
                        { "ResponseText",new ActionResponse()
                        {
                            ActionStatus = "Failed",
                            RequestMessage = RequestMessage,
                            SerialKey = RequestMessage.SerialKey,
                           StatusText = err
                        }.ToJson()}
                    }, r => { }, e => { }).Wait();               
            }
        );
         
    }


    public string Request(string RequestText)
    {
        Console.WriteLine("Requesting: \n" + RequestText);
        ActionRequest RequestMessage = DeserializeRequest(RequestText);
        ActionResponse ResponseMessage = null;
        
        string ResponseText = JObject.FromObject(ResponseMessage).ToString();
        Clients.Caller.SendAsync("Response", ResponseText);
        return ResponseText;
    }

    public string Response(string ResponseText)
    {
        Console.WriteLine("Получен результат выполнения удалённой процедуры: \n" + ResponseText);
        ActionResponse ActionResponse = DeserializeResponse(ResponseText);

        HandleResponse(ActionResponse);
        return ResponseText;
    }

    private void HandleResponse(ActionResponse actionResponse)
    {
        Console.WriteLine("Обрабатываем результат выполнения удалённой процедуры: \n" + actionResponse.RequestMessage.ActionName);
        var handler = connectionsContext.Take(actionResponse.SerialKey);
        if (actionResponse.ActionStatus == "Success")
        {
            handler.OnSuccess(actionResponse.MessageObject);
        }
        else
        {
            handler.OnError(actionResponse.StatusText);
        }
    }

    private ActionResponse ExecuteRequest(ActionRequest RequestMessage)
    {
        try
        {
            //Console.WriteLine($"Выполняем процедуру: {RequestMessage.ActionName}");

            object InvokationResult = actionsContext.DoAction(RequestMessage.ActionName, RequestMessage.MessageObject);
            if (InvokationResult is IAsyncResult ar)
            {
                ((dynamic)ar).Wait();
                InvokationResult = ((dynamic)ar).Result;
            }
            return new ActionResponse()
            {
                ActionStatus = "Success",
                SerialKey = RequestMessage.SerialKey,
                RequestMessage = RequestMessage,
                MessageObject = InvokationResult
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse()
            {
                ActionStatus = "Failed",
                SerialKey = RequestMessage.SerialKey,
                RequestMessage = RequestMessage,
                StatusText = ex.Message
            };
        }
    }


    private void ExecuteRequest(ActionRequest RequestMessage, Action<object> onsuccess, Action<string> onerror)
    {        
        actionsContext.DoAction(RequestMessage.ActionName, RequestMessage.MessageObject, onsuccess, onerror);         
    }

    private static ActionRequest DeserializeRequest(string json)
    {
        try
        {
            //return JsonConvert.DeserializeObject<ActionRequest>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Не удалось сериализовать сообщение с параметрами запроса: " + ex.Message);
            throw;
        }
    }

    private static ActionResponse DeserializeResponse(string json)
    {
        try
        {
            //return JsonConvert.DeserializeObject<ActionResponse>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Не удалось сериализовать сообщение с параметрами запроса: " + ex.Message);
            throw;
        }
    }
}
