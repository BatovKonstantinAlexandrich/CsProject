using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GrpcPort.Contracts.Actions;

public class ActionResponse
{
    public string SerialKey { get; set; } = "";
    public object? MessageObject { get; set; }
    public string ActionStatus { get; set; } = "Success";
    public string? StatusText { get; set; }
    public ActionRequest? RequestMessage { get; set; }
    public bool Confirmed { get; set; }

    public string ToJson() => JsonConvert.SerializeObject(this);

    public static ActionResponse FromJson(string json) =>
        JsonConvert.DeserializeObject<ActionResponse>(json)
        ?? throw new InvalidOperationException("Некорректный ActionResponse JSON.");

    public T? GetResult<T>()
    {
        if (MessageObject is null) return default;
        if (MessageObject is T typed) return typed;
        if (MessageObject is JToken token) return token.ToObject<T>();
        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(MessageObject));
    }
}
