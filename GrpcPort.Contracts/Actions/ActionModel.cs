using WinFormsApp.Console.Console_ClassModule.ClassDataModel;

namespace GrpcPort.Contracts.Actions;

/// <summary>Локальный исполняемый action на стороне клиента-владельца.</summary>
public class ActionModel : MethodModel
{
    private readonly Func<Dictionary<string, string>, object?> _todo;

    public ActionModel(string name, Func<Dictionary<string, string>, object?> todo)
    {
        Name = name;
        _todo = todo;
    }

    public object? Exec(Dictionary<string, string> args) => _todo(args);
}
