using System.Text;
using GrpcPort.Contracts;
using GrpcPort.Contracts.Actions;
using GrpcPort.Server.Sessions;
using Newtonsoft.Json;

using WinFormsApp.Console.Console_ClassModule.ClassDataModel;

namespace GrpcPort.Server.Actions;

/// <summary>
/// Маршрутизация publish / request / response поверх Inbound/Outbound очередей сессий.
/// Принцип обмена StreamEnvelope не меняется.
/// </summary>
public sealed class ActionBroker
{
    private readonly ActionsContext _actions;
    private readonly SessionManager _sessions;
    private readonly AsyncContext _pending = new();
    private readonly ILogger<ActionBroker> _logger;

    public ActionBroker(
        ActionsContext actions,
        SessionManager sessions,
        ILogger<ActionBroker> logger)
    {
        _actions = actions;
        _sessions = sessions;
        _logger = logger;
    }

    public async Task HandleInboundAsync(
        ClientSession session,
        StreamEnvelope envelope,
        string plainText,
        CancellationToken cancellationToken)
    {
        var type = envelope.Type ?? StreamActionTypes.Data;

        switch (type)
        {
            case StreamActionTypes.Ping:
                await SendAsync(session, StreamActionTypes.Pong, "pong", envelope.MessageId, cancellationToken);
                break;

            case StreamActionTypes.Publish:
                await HandlePublishAsync(session, plainText, cancellationToken);
                break;

            case StreamActionTypes.Unpublish:
                HandleUnpublish(session, plainText);
                await NotifyOthersAsync(session.ClientId, StreamActionTypes.OnUnpublish, plainText, cancellationToken);
                break;

            case StreamActionTypes.Request:
                HandleRequest(session, plainText);
                break;

            case StreamActionTypes.Response:
                HandleResponse(plainText);
                break;

            case StreamActionTypes.Catalog:
                await SendCatalogAsync(session, cancellationToken);
                break;

            default:
                _logger.LogDebug("[{Client}] data: {Text}", session.ClientId, plainText);
                break;
        }
    }

    public void OnSessionClosed(string clientId)
    {
        var removed = _actions.GetActions()
            .Where(a => a.OwnerClientId == clientId)
            .Select(a => a.Name)
            .ToList();

        _actions.RemoveByOwner(clientId);
        foreach (var name in removed)
            _logger.LogInformation("Action {Action} снят: клиент {Client} отключился", name, clientId);
    }

    private async Task HandlePublishAsync(
        ClientSession session,
        string methodJson,
        CancellationToken cancellationToken)
    {
        var model = MethodModel.FromJson(methodJson)
            ?? throw new InvalidOperationException("Некорректный MethodModel.");

        if (string.IsNullOrWhiteSpace(model.Name))
            throw new InvalidOperationException("MethodModel.Name пуст.");

        model.OwnerClientId = session.ClientId;
        var actionName = model.Name;
        var ownerId = session.ClientId;

        _actions.RemoveAction(actionName);
        _actions.AddAction(actionName, model, ownerId, (args, onSuccess, onError) =>
        {
            _ = ProxyToOwnerAsync(ownerId, actionName, args, onSuccess, onError);
        });

        session.PublishedActions.Add(actionName);
        _logger.LogInformation("Publish {Action} owner={Owner}", actionName, ownerId);

        await NotifyOthersAsync(ownerId, StreamActionTypes.OnPublish, model.ToJson(), cancellationToken);
        await SendAsync(session, StreamActionTypes.Data, $"published:{actionName}", null, cancellationToken);
    }

    private void HandleUnpublish(ClientSession session, string methodJson)
    {
        var model = MethodModel.FromJson(methodJson);
        if (model is null || string.IsNullOrWhiteSpace(model.Name))
            return;

        _actions.RemoveAction(model.Name);
        session.PublishedActions.Remove(model.Name);
        _logger.LogInformation("Unpublish {Action} owner={Owner}", model.Name, session.ClientId);
    }

    private void HandleRequest(ClientSession caller, string requestJson)
    {
        var request = requestJson.FromJson<ActionRequest>();
        var callerSerialKey = request.SerialKey;

        _logger.LogInformation(
            "Request {Action} from {Caller} key={Key}",
            request.ActionName,
            caller.ClientId,
            Truncate(callerSerialKey));

        if (string.Equals(request.ActionName, "ListActions", StringComparison.OrdinalIgnoreCase))
        {
            _ = ReplyToCallerAsync(caller, callerSerialKey, request, _actions.GetActions(), null);
            return;
        }

        _actions.DoAction(
            request.ActionName,
            request.MessageObject ?? new Dictionary<string, object>(),
            result => _ = ReplyToCallerAsync(caller, callerSerialKey, request, result, null),
            error => _ = ReplyToCallerAsync(caller, callerSerialKey, request, null, error));
    }

    private void HandleResponse(string responseJson)
    {
        var response = responseJson.FromJson< ActionResponse>();
        var handler = _pending.Take(response.SerialKey);
        if (handler is null)
        {
            _logger.LogWarning("Response без pending handler: {Key}", Truncate(response.SerialKey));
            return;
        }

        if (response.ActionSuccess)
            handler.OnSuccess(response.ActionResult);
        else
            handler.OnError(response.ErrorMessage ?? "Failed");
    }

    private async Task ProxyToOwnerAsync(
        string ownerClientId,
        string actionName,
        Dictionary<string, object> args,
        Action<object?> onSuccess,
        Action<string> onError)
    {
        if (!_sessions.TryGetByClientId(ownerClientId, out var owner) || owner is null || !owner.IsStreamOpen)
        {
            onError($"Владелец действия [{actionName}] не подключён.");
            return;
        }

        var hubKey = _pending.Put(new ActionHandler
        {
            OnSuccess = onSuccess,
            OnError = onError
        });

        var ownerRequest = new ActionRequest
        {
            SerialKey = hubKey,
            ActionName = actionName,
            MessageObject = args,
            AccessToken = owner.SessionToken
        };

        var envelope = CreateEnvelope(owner, StreamActionTypes.Request, ownerRequest.ToJson());
        await owner.Outbound.Writer.WriteAsync(envelope);
    }

    private async Task ReplyToCallerAsync(
        ClientSession caller,
        string serialKey,
        ActionRequest request,
        object? result,
        string? error)
    {
        try
        {
            var response = new ActionResponse
            {
                SerialKey = serialKey,
                RequestMessage = request,
                ActionSuccess = error is null,
                ErrorMessage = error,
                ActionResult = result
            };

            await SendAsync(caller, StreamActionTypes.Response, response.ToJson(), null, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Не удалось ответить клиенту {Client}", caller.ClientId);
        }
    }

    private async Task NotifyOthersAsync(
        string exceptClientId,
        string type,
        string payload,
        CancellationToken cancellationToken)
    {
        foreach (var session in _sessions.GetAll().Where(s => s.IsStreamOpen && s.ClientId != exceptClientId))
        {
            await SendAsync(session, type, payload, null, cancellationToken);
        }
    }

    private async Task SendCatalogAsync(ClientSession session, CancellationToken cancellationToken)
    {
        var json = JsonConvert.SerializeObject(_actions.GetActions());
        await SendAsync(session, StreamActionTypes.Catalog, json, null, cancellationToken);
    }

    private static Task SendAsync(
        ClientSession session,
        string type,
        string payload,
        string? correlationId,
        CancellationToken cancellationToken)
    {
        var envelope = CreateEnvelope(session, type, payload, correlationId);
        return session.Outbound.Writer.WriteAsync(envelope, cancellationToken).AsTask();
    }

    private static StreamEnvelope CreateEnvelope(
        ClientSession session,
        string type,
        string payload,
        string? correlationId = null)
    {
        var envelope = new StreamEnvelope
        {
            SessionToken = session.SessionToken,
            MessageId = Guid.NewGuid().ToString("N"),
            CorrelationId = correlationId ?? "",
            TimestampUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Type = type,
            Payload = Google.Protobuf.ByteString.CopyFrom(Encoding.UTF8.GetBytes(payload))
        };
        envelope.Headers["source"] = "action-broker";
        return envelope;
    }

    private static string Truncate(string? value) =>
        string.IsNullOrEmpty(value) ? "-" : value.Length <= 8 ? value : value[..8];
}
