using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

/// <summary>
/// Модель вызова функции удалённой процедуры
/// </summary>
public class ActionModel: MethodModel
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Method { get; set; }
    private readonly Func<Dictionary<string, string>, object> Todo;

    public ActionModel(string name, Func<Dictionary<string, string>, object> todo)
    {
        Name = name;
        Todo = todo;
    }

    public async Task<object> Exec(Dictionary<string, string> args)
    {
        Console.WriteLine($"[{nameof(ActionModel)}] Name={Name} выполнение процедуры");
        return Todo(args);
    }
    
}