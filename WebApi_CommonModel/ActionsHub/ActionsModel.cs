using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using System.Threading;
using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

public class ActionsModel: MethodModel
{

    [System.Text.Json.Serialization.JsonIgnore]
    [JsonIgnore]
    public Action<Dictionary<string, object>, Action<object>, Action<string>> Todo;

    public ActionsModel(string name, Action<Dictionary<string, object>, Action<object>, Action<string>> todo)
    {
        Name = name;
        Todo = todo;
    }
     
    public object Exec(Dictionary<string, object> args)
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
        while(!completed)
            Thread.Sleep(100);
        return result;
    }

    public void Exec(Dictionary<string, object> args, Action<object> onsuccess, Action<string> onerror)
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
 