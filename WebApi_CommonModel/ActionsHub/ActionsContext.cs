using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

using Microsoft.AspNetCore.SignalR;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Контекст регистрации служебных функций
/// </summary>
public class ActionsContext
{
    public class ActionResponse
    {
        public string SerialKey { get; set; }
        public object MessageObject { get; set; }
        public string ActionStatus { get; set; }
        public string StatusText { get; set; }
        public ActionRequest RequestMessage { get; set; }
        public bool Confirmed { get; set; }
    }


    /// <summary>
    /// Модель запроса "Удалённый вызов процедуры"
    /// </summary>
    public class ActionRequest
    {


        /// <summary>
        /// Ключ доступа клиента
        /// </summary>
        //[JsonProperty("token")]
        public string AccessToken { get; set; }


        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        //[JsonProperty("mid")]
        public string SerialKey { get; set; }


        /// <summary>
        /// Имя процедуры
        /// </summary>
        //[JsonProperty("request.path")]
        public string ActionName { get; set; }


        /// <summary>
        /// Параметры выполнения
        /// </summary>
        //[JsonProperty("request.pars")]
        public Dictionary<string, string> MessageObject { get; set; }


    }
    public class ActionModel : MethodModel
    {

        [System.Text.Json.Serialization.JsonIgnore]
        [JsonIgnore]
        public Action<Dictionary<string, string>, Action<object>, Action<string>> Todo;

        public ActionModel(string name, Action<Dictionary<string, string>, Action<object>, Action<string>> todo)
        {
            Name = name;
            Todo = todo;
        }

        public object Exec(Dictionary<string, string> args)
        {
            bool completed = false;
            object result = null;
            Todo(args,
                resp => {
                    result = resp;
                    completed = true;
                },
                err => {
                    result = err;
                    completed = true;
                });
            while (!completed)
                Thread.Sleep(100);
            return result;
        }

        public void Exec(Dictionary<string, string> args, Action<object> onsuccess, Action<string> onerror)
        {
            Todo(args,
                resp => {
                    onsuccess(resp);
                },
                err => {
                    onerror(err);
                });

        }
    }


    /// <summary>
    /// Операции по наименованиям
    /// </summary>
    private ConcurrentDictionary<string, ActionModel> Actions = new ConcurrentDictionary<string, ActionModel>();
    

    public List<MethodModel> GetActions() => Actions.Values.Select(a => (MethodModel)a).ToList();

    public ActionsContext.ActionModel GetActionModel(string name) => Actions[name];

    public ActionsContext()
    {        
        this.AddAction(nameof(GetActionList), new MethodModel()
        {
            Label = "Получить список операции",
            Name = nameof(GetActionList)
        }, (args, onsuccess, onerror) =>
        {
            onsuccess(GetActions());
        });
        this.AddAction("Ping", new MethodModel()
        {
            Label = "Получить список операции",
            Name = "Ping"
        }, (args, onsuccess, onerror) =>
        {
            onsuccess("ok");
        });
        this.AddAction(nameof(GetActionModel), new MethodModel()
        {
            Label = "Получить модель параметров операции",
            Name = nameof(GetActionModel)
        }, (args, onsuccess,onerror) =>
        {
            if (!args.ContainsKey("action"))
                throw new ArgumentException("action");
            onsuccess(GetActionModel(args["action"]));
        });
    }

    /// <summary>
    /// Регистрация операции
    /// </summary>
    public Action AddAction(string Name, MethodModel method, 
        Action<Dictionary<string,string>,Action<object>,Action<string>> Todo)
    {
        var action = new ActionModel(Name, Todo)
        {
            Label = method.Label,
            Name = method.Name,
            Parameters = method.Parameters,
            Attributes = method.Attributes
        };
        this.Actions[Name] = action;
        return () => {
            this.Actions.TryRemove(Name, out var removedAction);
        };
    }

  
    /// <summary>
    /// Выполнение операции
    /// </summary>
    public object DoAction(string Action, Dictionary<string,string> args)
    {
        if (Actions.ContainsKey(Action) == false)
        {
            string list = GetActionList();
            throw new Exception($"Операция [{Action}] не зарегистрирована. Доступны следующие операции: {list}");
        }
        var result = Actions[Action].Exec(args);
        if(result is Task t)
        {
            t.Wait();
            return ((dynamic)t).Result;
        }
        return result;
    }

    public void DoAction(string Action, Dictionary<string, string> args, Action<object> onsuccess, Action<string> onerror)
    {
        if (Actions.ContainsKey(Action) == false)
        {
            string list = GetActionList();
            throw new Exception($"Операция [{Action}] не зарегистрирована. Доступны следующие операции: {list}");
        }
        Actions[Action].Exec(args, onsuccess, onerror);        
    }


    /// <summary>
    /// Список операций
    /// </summary>
    /// <returns></returns>
    private string GetActionList()
    {
        string result = "";
        Actions.Keys.ToList().ForEach(k => result += "," + k);
        return Actions.Count() > 0 ? result.Substring(1) : "Нет операций";
    }
}
