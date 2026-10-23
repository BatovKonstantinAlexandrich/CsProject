using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinFormsApp.Console.Console_ClassModule.ClassDataModel;


/// <summary>
/// Хранит описания внешних операций сервиса.
/// Наполняется действиями в рнезультате обмена
/// </summary>
public class WebServiceContext
{

    /// <summary>
    /// Операции по наименованиям внешних сервисов
    /// </summary>
    private ConcurrentDictionary<string, ActionModel> Actions = new ConcurrentDictionary<string, ActionModel>();


    public WebServiceContext()
    {         
    }
    

    /// <summary>
    /// Список операций
    /// </summary>
    private string GetActionList()
    {
        string result = "";
        Actions.Keys.ToList().ForEach(k => result += "," + k);
        return Actions.Count() > 0 ? result.Substring(1) : "Нет операций";
    }


    /// <summary>
    /// Регистрация операции
    /// </summary>
    public void AddAction(string Name, ActionModel model)
    {
        System.Console.WriteLine($"Выполняем процедуру {nameof(AddAction)}");
        this.Actions[Name] = model;
    }


    /// <summary>
    /// Выполнение операции
    /// </summary>
    public object DoAction(string Action, Dictionary<string, string> args)
    {
        System.Console.WriteLine($"Выполняем процедуру {Action}");
        if (Actions.ContainsKey(Action) == false)
        {
            string list = GetActionList();
            throw new System.Exception($"Операция [{Action}] не зарегистрирована. Доступны следующие операции: {list}");
        }
        var result = Actions[Action].Exec(args);
        if (result is Task)
        {
            result.Wait();
            return result.Result;
        }
     
        return result;
    }

    public void RemoveAction(string Action)
    {
        if (Actions.ContainsKey(Action))
        {
            Actions.TryRemove(Action , out var p);
        }
    }
    
}