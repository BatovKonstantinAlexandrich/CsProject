using GrpcPort.Contracts.Actions;
using Newtonsoft.Json;

using WinFormsApp.Console.Console_ClassModule.ClassDataModel;

namespace GrpcPort.Server.Actions;

public sealed class HubActionModel : MethodModel
{
    [JsonIgnore]
    public Action<Dictionary<string, object>, Action<object?>, Action<string>> Todo { get; }

    public HubActionModel(
        string name,
        Action< Dictionary<string, object>, Action<object?>, Action<string>> todo)
    {
        Name = name;
        Todo = todo;
    }

    public void Exec(
        Dictionary<string, object> args,
        Action<object?> onSuccess,
        Action<string> onError) =>
        Todo(args, onSuccess, onError);
}
