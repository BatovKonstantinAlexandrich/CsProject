using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

using Microsoft.AspNetCore.SignalR;
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
    /// <summary>
    /// Операции по наименованиям
    /// </summary>
    private ConcurrentDictionary<string, ActionModel> Actions = new ConcurrentDictionary<string, ActionModel>();
    

    public List<MethodModel> GetActions() => Actions.Values.Select(a => (MethodModel)a).ToList();

    public ActionModel GetActionModel(string name) => Actions[name];

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
