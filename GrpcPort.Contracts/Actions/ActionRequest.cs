using Newtonsoft.Json;

namespace GrpcPort.Contracts.Actions;

public class ActionRequest
{
    public string? AccessToken { get; set; }
    public string SerialKey { get; set; } = "";
    public string ActionName { get; set; } = "";
    public Dictionary<string, string> MessageObject { get; set; } = new();
    /// <summary>Клиент, ожидающий ответ (заполняет сервер при проксировании).</summary>
    public string? ReplyToClientId { get; set; }
    public string? HubSerialKey { get; set; }

    public string ToJson() => JsonConvert.SerializeObject(this);

    public static ActionRequest FromJson(string json) =>
        JsonConvert.DeserializeObject<ActionRequest>(json)
        ?? throw new InvalidOperationException("Некорректный ActionRequest JSON.");
}
