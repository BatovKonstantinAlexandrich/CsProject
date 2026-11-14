using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Объект реализует управление клиентом ассоциированным с соединением
/// </summary>
public class ConsumerController
{
    private string Token { get; set; }


    /// <summary>
    /// Наименование сервиса
    /// </summary>
    public string Name { get; set; }


    /// <summary>
    /// Передача запроса на подключенный сервис
    /// </summary>
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    private Action<string> Send { get; set; }

    /// <summary>
    /// Конеткст регистрации операци
    /// </summary>
    public AsyncContext AsyncContext { get; }

    /// <summary>
    /// Включено логировани
    /// </summary>
    private readonly bool _Logging = true;

    /// <summary>
    /// Зарегистрированные операции
    /// </summary>
    public Dictionary<string, HubActionModel> Actions { get; set; } = new Dictionary<string, HubActionModel>();

    /// <summary>
    /// Операции для отмены изменений
    /// </summary>
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public List<Action> Rollback { get; set; } = new();


    public ConsumerController(Action<string> Send, AsyncContext AsyncContext)
    {
        this.Send = Send;
        this.AsyncContext = AsyncContext;
    }


    /// <summary>
    /// Передача запроса на выполнение операции
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="Args"></param>
    /// <param name="OnSuccess"></param>
    /// <param name="OnError"></param>
    /// <returns></returns>
    public async Task Request(
        string Action,
        Dictionary<string, string> Args,
        Action<object> OnSuccess,
        Action<string> OnError = null)
    {
        if (_Logging)
            System.Console.WriteLine($"Service: {Name}. Requesting: \n" + JObject.FromObject(Args).ToString());
        string SerialKey = this.AsyncContext.Put(new ()
        {
            OnSuccess = OnSuccess,
            OnError = OnError == null ? (evt) => {

            }: OnError
        });
        var RequestMessage = new ActionRequest()
        {
            SerialKey = SerialKey,
            MessageObject = Args,
            ActionName = Action,
            AccessToken = Token
        };
        string RequestText = JObject.FromObject(RequestMessage).ToString();
        Send(RequestText);
    }



    /// <summary>
    /// Регистрация операции по заданной модели 
    /// </summary>
    public Action AddAction(string action, MethodModel methodModel)
    {
        Actions[action] = new HubActionModel(action, (args,onsuccess,onerror) =>
        {
            Request(action, args,
                (resp) => {
                    onsuccess(resp);
                },
                (err) => {
                    onerror(err);
                }
            ).Wait();
        })
        {
             Attributes = methodModel.Attributes,
             Parameters = methodModel.Parameters,
             Name = methodModel.Name,
             Label = methodModel.Label
        };
        return () => {
            if(Actions.ContainsKey(action))
                Actions.Remove(action);
        };
    }

    
}