using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


/// <summary>
/// Модель вызова функции удалённой процедуры
/// </summary>
public class ActionModel
{
    public string Name { get; set; }
    public string Method { get; set; }


    private readonly Func<Dictionary<string, string>, object> Todo;

    public ActionModel(string name, Func<Dictionary<string, string>, object> todo)
    {
        Name = name;
        Todo = todo;
    }

    public Task<object> Exec(Dictionary<string, string> args)
    {
        Console.WriteLine($"[{nameof(ActionModel)}] Name={Name} выполнение процедуры");
        return Task.FromResult(Todo(args));
    }
    
}