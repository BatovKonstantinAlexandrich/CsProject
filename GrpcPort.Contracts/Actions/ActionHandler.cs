namespace GrpcPort.Contracts.Actions;

public class ActionHandler
{
    public Action<object?> OnSuccess { get; set; } = _ => { };
    public Action<string> OnError { get; set; } = _ => { };
}
