using System.Collections.Concurrent;

namespace GrpcPort.Contracts.Actions;

/// <summary>Реестр локальных ActionModel на клиенте.</summary>
public class WebServiceContext
{
    private readonly ConcurrentDictionary<string, ActionModel> _actions = new();

    public IReadOnlyCollection<string> Names => _actions.Keys.ToArray();

    public void AddAction(string name, ActionModel model) => _actions[name] = model;

    public void RemoveAction(string name) => _actions.TryRemove(name, out _);

    public bool Contains(string name) => _actions.ContainsKey(name);

    public object? DoAction(string action, Dictionary<string, string> args)
    {
        if (!_actions.TryGetValue(action, out var model))
        {
            var list = _actions.Count > 0 ? string.Join(", ", _actions.Keys) : "Нет операций";
            throw new InvalidOperationException(
                $"Операция [{action}] не зарегистрирована. Доступны: {list}");
        }

        var result = model.Exec(args);
        if (result is Task task)
        {
            task.GetAwaiter().GetResult();
            var resultProp = task.GetType().GetProperty("Result");
            return resultProp?.GetValue(task);
        }

        return result;
    }
}
