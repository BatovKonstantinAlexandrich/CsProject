using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;
using Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;


/// <summary>
/// Клиент RPC к ActionsHub
/// </summary>
public class ActionsConsumer : ActionsContext2
{

    
    public string Token { get; set; }
    public string URL { get; set; }

    public bool _Logging = true;

    public WebServiceContext _serviceContext = new WebServiceContext();

    public ConcurrentQueue<ActionResponse> MessageQueue = new();

    public Action<MethodModel> OnPublish { get; set; } = p => { };
    public Action<MethodModel> OnUnPublish { get; set; } = p => { };


    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public Action<string> OnStateChanged { get; set; } = state => { };

    
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public Action OnConnected { get; set; } = () => { };

    
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public Action<Exception> OnDisconnected { get; set; } = _ => { };

    public HubConnection Connection;

    public ActionsConsumer()
    {
        this.URL = URL;
        this.OnStateChanged += state => {
            if (this._Logging)
            {
                this.Info($"StateChanged: {state}");
            }
        };
    }
    public ActionsConsumer(string URL)
    {
        this.URL = URL;
        this.OnStateChanged += state => {
            if (this._Logging)
            {
                this.Info($"StateChanged: {state}");
            }
        };
    }

    public async Task<bool> IsConnected()
    {
        return this.Connection is not null && this.Connection.State == HubConnectionState.Connected;
    }

    public async Task Connect()
    {
        var builder = new HubConnectionBuilder();
        builder.WithUrl(this.URL = URL/*, options =>
        {
            // 1. Добавляем клиентский сертификат
            var cert = new X509Certificate2("client.pfx", "password");
            options.ClientCertificates.Add(cert);

            // 2. Критично: настраиваем обработчик для HTTP-запросов (при необходимости)
            options.HttpMessageHandlerFactory = (handler) =>
            {
                if (handler is HttpClientHandler clientHandler)
                {
                    // Разрешаем использование самоподписанных сертификатов на клиенте (для тестов)
                    clientHandler.ServerCertificateCustomValidationCallback +=
                        (sender, certificate, chain, sslPolicyErrors) => true;
                }
                return handler;
            };

            // 3. Критично: настраиваем обработчик для WebSockets
            options.WebSocketConfiguration = (sockets) =>
            {
                sockets.RemoteCertificateValidationCallback = (sender, certificate, chain, policyErrors) => true;
            };
        }*/);
        builder.WithAutomaticReconnect();
        this.Connection = builder.Build();
        
        Connection.Closed += ex =>
        {
            try
            {
                OnDisconnected?.Invoke(ex);
            }
            catch
            {
            }
            return Task.CompletedTask;
        };

        this.OnStateChanged("Устанавливаем соединение: " + URL);
        await Connection.StartAsync().ContinueWith((result) => {
            if (Connection.State != HubConnectionState.Connected)
            {
                this.OnStateChanged("Соединение не установлено: " + URL);
                Thread.Sleep(1000);
                this.OnStateChanged("Ожидание 3 сек");
                Thread.Sleep(1000);
                this.OnStateChanged("Ожидание 2 сек");
                Thread.Sleep(1000);
                this.OnStateChanged("Ожидание 1 сек");
                return Connect();
            }
            else
            {
                this.OnStateChanged("Соединение установлено: " + URL);
                Connection.On<string>("Response", OnResponse);
                Connection.On("Ping", OnPing);
                Connection.On<string>("Request", OnRequest);

                InitServiceContext();

                return Task.Run(() => {
                    OnConnected?.Invoke();
                });
            }
        });

        if (Connection.State != HubConnectionState.Connected)
        {
            OnConnected?.Invoke();
        }
         
    }

    private void InitServiceContext()
    {
        var model = new ActionModel("Response", (args) =>
        {
            this.OnResponse($"{args["ResponseText"]}");
            return args;
        })
        {
            Attributes = new(),
            Parameters = new()
        };
        _serviceContext.AddAction(model.Name, model);

        var ctrl = this;
        /*foreach(string action in Public)
        {
            var method = this.GetType().GetMethod(action);
            Connection.On<object>(action, (args)=> {
                method.Invoke(ctrl, new object[] { args.ToString() });
            });
        }*/

        Task.Run(() =>
        {
            while (Connection.State == HubConnectionState.Connected)
            {
                MessageQueue.TryDequeue(out var msg);
                if (msg is not null)
                {
                    Request("Response", new() { { "RequestText", msg.ToJson() } }, p => { }, e => { }).Wait();
                }
                Thread.Sleep(100);
            }
        });
    }

    public async Task<Dictionary<string, List<MethodModel>>> ListClientActions()
    {
        return await Connection.InvokeAsync<Dictionary<string, List<MethodModel>>>("ListClientActions");
    }

    
    public async Task OnRequest(string RequestText)
    {
        Console.WriteLine("Получено тестовое сообщение: \n" + RequestText);
        ActionRequest RequestMessage = Deserrialize(RequestText);
        ActionResponse ResponseMessage = null;
        //if (String.IsNullOrWhiteSpace(RequestMessage.AccessToken))
        //{
        //    ResponseMessage = new ActionResponse()
        //    {
        //        ActionStatus = "Failed",
        //        SerialKey = RequestMessage.SerialKey,
        //        RequestMessage = RequestMessage,
        //        StatusText = "Нет доступа"
        //    };
        //}
        //else
        //{
        //    ResponseMessage = ExecuteRequest(RequestMessage);
        //}

        ResponseMessage = ExecuteRequest(RequestMessage);

        string ResponseText = ResponseMessage.ToJson();

        Console.WriteLine("Отправляем результат выполнения процедуры: \n" + ResponseText);
        //MessageQueue.Enqueue(ResponseMessage);
        Connection.InvokeAsync("Response", ResponseText).Wait();
    }

    private ActionResponse ExecuteRequest(ActionRequest RequestMessage)
    {
        try
        {
            if (_Logging)
                Console.WriteLine("Обрабатываем запрос на выполнение процедуры: " + RequestMessage.ActionName);
            object InvokationResult = _serviceContext.DoAction(RequestMessage.ActionName, RequestMessage.MessageObject);
            return new ActionResponse()
            {
                ActionSuccess = true,
                SerialKey = RequestMessage.SerialKey,
                RequestMessage = RequestMessage,
                ActionResult = InvokationResult
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse()
            {
                ActionSuccess = false,
                SerialKey = RequestMessage.SerialKey,
                RequestMessage = RequestMessage,
                ErrorMessage = ex.Message
            };
        }
    }
    private static ActionRequest Deserrialize(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<ActionRequest>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Не удалось сериализовать сообщение с параметрами запроса: " + ex.Message);
            throw;
        }
    }

    private void OnPing()
    {
        Request("Test", new Dictionary<string, object>() { },
            (resp) => {
                Console.WriteLine(resp);
            },
            (error) => {
                Console.WriteLine(error);
            }
        );
    }

    public void Configure(HttpConnectionOptions options)
    {
        options.SkipNegotiation = true;
        options.Transports = HttpTransportType.WebSockets;
    }


    private void OnResponse(string ResponseText)
    {
        if (_Logging)
            Console.WriteLine("Handling Response: \n" + ResponseText);
        ActionResponse ResponseMessage = JsonConvert.DeserializeObject<ActionResponse>(ResponseText);
        var Handler = Take(ResponseMessage.SerialKey);
        if (ResponseMessage.ActionSuccess)
        {
            if (_Logging)
                Console.WriteLine("Handled Response: \n" + ResponseText);
            Handler.OnSuccess(ResponseMessage.ActionResult);
        }
        else
        {
            Handler.OnError(ResponseMessage.ErrorMessage);
            Console.WriteLine("Операция не выполнена: " + ResponseMessage.SerialKey);
        }
    }


    public async Task PublishClassModel(ClassModel classModel)
    {

        await Connection.InvokeAsync("PublishClassModel", classModel.ToJson());
    }

    public async Task Publish(MethodModel method, Func<Dictionary<string, object>, object> ExecuteRequest)
    {
        var model = new ActionModel(method.Name, ExecuteRequest)
        {
            Attributes = method.Attributes,
            Parameters = method.Parameters,
            Icon = method.Icon,
            Label = method.Label,
            Description = method.Description,
            Type = method.Type
        };
        _serviceContext.AddAction(method.Name, model);
        var methodJson = method.ToJson();
        await Connection.InvokeAsync("Publish", methodJson);
    }

    public async Task UnPublish(MethodModel method)
    {
        _serviceContext.RemoveAction(method.Name);        
        var methodJson = method.ToJson();
        await Connection.InvokeAsync("UnPublish", methodJson);
    }


    public async Task Request(
                    string Action,
                    Dictionary<string, object> Args,
                    Action<object> OnSuccess,
                    Action<string> OnError = null)
    {
        if (_Logging)
            Console.WriteLine("Requesting: \n" + JObject.FromObject(Args).ToString());
        if (Connection == null)
        {
            await Connect();
        }


        string SerialKey = Put(new ActionHandler()
        {
            OnSuccess = OnSuccess,
            OnError = OnError == null ? (evt) => {
            }
            : OnError
        });
        var RequestMessage = new ActionRequest()
        {
            SerialKey = SerialKey,
            MessageObject = Args,
            ActionName = Action,
            AccessToken = Token
        };
        string RequestText = JObject.FromObject(RequestMessage).ToString();
        if (Connection != null)
        {
            await Connection.InvokeAsync("Request", RequestText);
            if (_Logging)
                Console.WriteLine("Requested: \n" + RequestText);

        }
    }


    public async Task SendRequest(
                    string Action,
                    Dictionary<string, object> Args,
                    Action<object> OnSuccess,
                    Action<string> OnError = null)
    {
        if (_Logging)
            Console.WriteLine("Requesting: \n" + JObject.FromObject(Args).ToString());
        if (Connection == null)
        {
            await Connect();
        }


        string SerialKey = Put(new ActionHandler()
        {
            OnSuccess = OnSuccess,
            OnError = OnError == null ? (evt) => {
            }
            : OnError
        });
        var RequestMessage = new ActionRequest()
        {
            SerialKey = SerialKey,
            MessageObject = Args,
            ActionName = Action,
            AccessToken = Token
        };
        string RequestText = JObject.FromObject(RequestMessage).ToString();
        if (Connection != null)
        {
            await Connection.InvokeAsync("SendRequest", RequestText);
            if (_Logging)
                Console.WriteLine("Requested: \n" + RequestText);

        }
    }


    public async Task RequestData<TResponse>(
                    string Action,
                    Dictionary<string, object> Args,
                    Action<MethodResult<TResponse>> OnSuccess,
                    Action<string> OnError = null)
        where TResponse : class
    {
        if (_Logging)
            Console.WriteLine("Requesting: \n" + JObject.FromObject(Args).ToString());
        if (Connection == null)
        {
            await Connect();
        }


        string SerialKey = Put(new ActionHandler()
        {
            OnSuccess = response => {
                if (response is not null)
                {
                    var responseData = response.ToJson().FromJson<TResponse>();
                    OnSuccess(MethodResult<TResponse>.OnResult(responseData));
                }
                else
                {
                    OnSuccess(MethodResult<TResponse>.OnError("Получена ссылка на null"));
                }
            },
            OnError = (evt) => {
                OnSuccess(MethodResult<TResponse>.OnError(evt));
            }
        });
        var RequestMessage = new ActionRequest()
        {
            SerialKey = SerialKey,
            MessageObject = Args,
            ActionName = Action,
            AccessToken = Token
        };
        string RequestText = JObject.FromObject(RequestMessage).ToString();
        if (Connection != null)
        {
            await Connection.InvokeAsync("Request", RequestText);
            if (_Logging)
                Console.WriteLine("Requested: \n" + RequestText);

        }
    }


    public async Task InvokeAsync(string action, params object[] args)
    {
        Console.WriteLine($"InvokeAsync({action}, ... )");
        if (Connection == null || Connection.State != HubConnectionState.Connected)
        {
            throw new Exception("Is not connected now");
        }
        else
        {
            await Connection.InvokeAsync(action, args);
        }

    }

    public Action RegisterServiceReference(object service)
    {
        var methods = service.GetType().GetOwnMethodNames();
        var classDbContext = new ClientClassDbContext();
        var classModel = classDbContext.CreateClassModel(service.GetType());

        this.Info($"{service.GetTypeName()} => {methods.ToJson()}");

        foreach (var method in classModel.Methods)
        {
            this.Info(method.Label);
            this.Publish(method, parameters =>
            {
                try
                {

                    var parameterNames = service.GetType().GetMethod(method.Name).GetParameters().Select(p => p.Name).ToList();
                    Dictionary<string,object> args = new();
                    int i = 0;
                    foreach (var parameter in parameterNames)
                    {
                        args[parameter] = parameters[parameter];
                    }
                    var result = service.Call(method.Name, args);
                    return result;
                }
                catch (Exception ex)
                {
                    this.OnError($"Исключение при регистрации сервисных операций типа {service.GetTypeName()}: {ex.Message} ");
                    throw;
                }
            });
        }
        return () => {
            foreach (var method in classModel.Methods)
            {
                this.UnPublish(method);
            }
        };
    }

    private Action<string> OnError { get; set; } = error => { };

    internal void UnregistreServiceReference(object service)
    {
        foreach (var methodName in service.GetType().GetOwnMethodNames())
        {
            this.Unpublish(methodName);
        }

    }

    private async Task Unpublish(string action)
    {
        _serviceContext.RemoveAction(action);
        await Connection.InvokeAsync("Unpublish", new object[] { action });
    }
}
