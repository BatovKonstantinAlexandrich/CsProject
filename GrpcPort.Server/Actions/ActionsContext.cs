using System.Collections.Concurrent;
using GrpcPort.Contracts.Actions;

using WinFormsApp.Console.Console_ClassModule.ClassDataModel;

namespace GrpcPort.Server.Actions;

/// <summary>Глобальный каталог опубликованных действий (аналог ActionsHub.ActionsContext).</summary>
public sealed class ActionsContext
{
    private readonly ConcurrentDictionary<string, HubActionModel> _actions = new();
    private readonly ConcurrentDictionary<string, string> _ownerByAction = new();

    public IReadOnlyCollection<MethodModel> GetActions() =>
        _actions.Values.Select(a => new MethodModel
        {
            Name = a.Name,
            Type = a.Type,
            Label = a.Label,
            Icon = a.Icon,
            Help = a.Help,
            Description = a.Description,
            OwnerClientId = a.OwnerClientId,
            Parameters = a.Parameters,
            Attributes = a.Attributes
        }).ToArray();

    public bool TryGet(string name, out HubActionModel? model) =>
        _actions.TryGetValue(name, out model);

    public string? GetOwner(string actionName) =>
        _ownerByAction.TryGetValue(actionName, out var owner) ? owner : null;

    public Action AddAction(
        string name,
        MethodModel method,
        string ownerClientId,
        Action<Dictionary<string, object>, Action<object?>, Action<string>> todo)
    {
        var action = new HubActionModel(name, todo)
        {
            Label = method.Label,
            Name = method.Name,
            Type = method.Type,
            Description = method.Description,
            Icon = method.Icon,
            Help = method.Help,
            Parameters = method.Parameters,
            Attributes = method.Attributes,
            OwnerClientId = ownerClientId
        };

        _actions[name] = action;
        _ownerByAction[name] = ownerClientId;

        return () =>
        {
            _actions.TryRemove(name, out _);
            _ownerByAction.TryRemove(name, out _);
        };
    }

    public void RemoveAction(string name)
    {
        _actions.TryRemove(name, out _);
        _ownerByAction.TryRemove(name, out _);
    }

    public void RemoveByOwner(string clientId)
    {
        foreach (var kv in _ownerByAction.Where(x => x.Value == clientId).ToList())
            RemoveAction(kv.Key);
    }

    public void DoAction(
        string action,
        Dictionary<string, object> args,
        Action<object?> onSuccess,
        Action<string> onError)
    {
        if (!_actions.TryGetValue(action, out var model))
        {
            var list = _actions.Count > 0 ? string.Join(", ", _actions.Keys) : "Нет операций";
            onError($"Операция [{action}] не зарегистрирована. Доступны: {list}");
            return;
        }

        model.Exec(args, onSuccess, onError);
    }
}
