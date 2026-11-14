using WinFormsApp.Console.Console_ClassModule.ClassDataModel;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcPort.Client.Crypto;
using GrpcPort.Contracts;
using GrpcPort.Contracts.Actions;
using Newtonsoft.Json;

namespace GrpcPort.Client.Actions;

/// <summary>
/// gRPC-аналог ActionsConsumer: auth + stream + Publish/Request/Response.
/// </summary>
public sealed class GrpcActionsClient : IAsyncDisposable
{
    private readonly string _serverAddress;
    private readonly string _clientId;
    private readonly string _secret;
    private readonly AsyncContext _async = new();
    private readonly ConcurrentDictionary<string, MethodModel> _remoteActions = new();

    private GrpcChannel? _channel;
    private PortService.PortServiceClient? _port;
    private AsyncDuplexStreamingCall<StreamEnvelope, StreamEnvelope>? _call;
    private X509Certificate2? _cert;
    private string? _sessionToken;
    private Task? _receiveTask;
    private CancellationTokenSource? _lifetime;

    public WebServiceContext ServiceContext { get; } = new();
    public bool Logging { get; set; } = true;
    public Action<MethodModel>? OnPublish { get; set; }
    public Action<MethodModel>? OnUnpublish { get; set; }
    public bool IsConnected => _call is not null && _sessionToken is not null;

    public GrpcActionsClient(string serverAddress, string clientId, string? secret = null)
    {
        _serverAddress = serverAddress;
        _clientId = clientId;
        _secret = secret ?? $"secret-{clientId}";
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        _lifetime = new CancellationTokenSource();
        _channel = GrpcChannel.ForAddress(_serverAddress, new GrpcChannelOptions
        {
            HttpHandler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true,
                KeepAlivePingDelay = TimeSpan.FromSeconds(30),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(10)
            }
        });
        _port = new PortService.PortServiceClient(_channel);

        AuthResponse? auth = null;
        for (var attempt = 1; attempt <= 20; attempt++)
        {
            try
            {
                auth = await _port.AuthenticateAsync(new AuthRequest
                {
                    ClientId = _clientId,
                    Secret = _secret
                }, cancellationToken: cancellationToken);
                break;
            }
            catch (RpcException ex) when (attempt < 20 &&
                ex.StatusCode is StatusCode.Unavailable or StatusCode.Internal or StatusCode.Cancelled)
            {
                Info($"сервер не готов ({ex.StatusCode}), попытка {attempt}/20...");
                await Task.Delay(500, cancellationToken);
            }
        }

        if (auth is null || !auth.Success)
            throw new InvalidOperationException(auth?.Message ?? "Не удалось авторизоваться.");

        _sessionToken = auth.SessionToken;
        _cert = X509CertificateLoader.LoadCertificate(auth.EncryptionCertificate.ToByteArray());
        Info($"auth OK, cert={auth.CertificateThumbprint}");

        _call = _port.OpenStream(cancellationToken: _lifetime.Token);
        _receiveTask = Task.Run(() => ReceiveLoopAsync(_lifetime.Token));

        // handshake: ping открывает поток (первое сообщение с session_token)
        await SendRawAsync(StreamActionTypes.Ping, "hello", encrypt: false, cancellationToken);
    }

    public async Task PublishAsync(
        MethodModel method,
        Func<Dictionary<string, object>, object?> execute,
        CancellationToken cancellationToken = default)
    {
        var model = new ActionModel(method.Name, execute)
        {
            Attributes = method.Attributes.ToJson().FromJson<List<Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel.MethodModelAttribute>>(),
            Parameters = method.Parameters.ToJson().FromJson<List<Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel.MethodParameterModel>>(),
            Icon = method.Icon,
            Label = method.Label,
            Description = method.Description,
            Type = method.Type
        };
        ServiceContext.AddAction(method.Name, model);
        await SendRawAsync(StreamActionTypes.Publish, method.ToJson(), encrypt: false, cancellationToken);
        Info($"Publish → {method.Name}");
    }

    public async Task UnpublishAsync(MethodModel method, CancellationToken cancellationToken = default)
    {
        ServiceContext.RemoveAction(method.Name);
        await SendRawAsync(StreamActionTypes.Unpublish, method.ToJson(), encrypt: false, cancellationToken);
    }

    public Task RequestAsync(
        string action,
        Dictionary<string, object> args,
        Action<object?> onSuccess,
        Action<string>? onError = null,
        CancellationToken cancellationToken = default)
    {
        var serialKey = _async.Put(new ActionHandler
        {
            OnSuccess = onSuccess,
            OnError = onError ?? (_ => { })
        });

        var request = new ActionRequest
        {
            SerialKey = serialKey,
            ActionName = action,
            MessageObject = args,
            AccessToken = _sessionToken
        };

        return SendRawAsync(StreamActionTypes.Request, request.ToJson(), encrypt: false, cancellationToken);
    }

    public Task<object?> RequestAsync(
        string action,
        Dictionary<string, object> args,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout ?? TimeSpan.FromSeconds(30));
        cts.Token.Register(() => tcs.TrySetException(new TimeoutException($"Timeout: {action}")));

        _ = RequestAsync(
            action,
            args,
            result => tcs.TrySetResult(result),
            error => tcs.TrySetException(new InvalidOperationException(error)),
            cancellationToken);

        return tcs.Task;
    }

    public IReadOnlyCollection<MethodModel> GetKnownRemoteActions() =>
        _remoteActions.Values.ToArray();

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_call is not null)
                await _call.RequestStream.CompleteAsync();
        }
        catch { /* ignore */ }

        _lifetime?.Cancel();
        if (_receiveTask is not null)
        {
            try { await _receiveTask.WaitAsync(TimeSpan.FromSeconds(2)); }
            catch { /* ignore */ }
        }

        _call?.Dispose();
        _cert?.Dispose();
        _channel?.Dispose();
        _lifetime?.Dispose();
    }

    private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var msg in _call!.ResponseStream.ReadAllAsync(cancellationToken))
            {
                var text = DecodePayload(msg);
                await HandleIncomingAsync(msg.Type, text);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (RpcException ex) when (ex.StatusCode is StatusCode.Cancelled or StatusCode.Unavailable)
        {
        }
        catch (Exception ex)
        {
            Info($"receive error: {ex.Message}");
        }
    }

    private async Task HandleIncomingAsync(string type, string text)
    {
        switch (type)
        {
            case StreamActionTypes.Request:
                await OnRequestAsync(text);
                break;

            case StreamActionTypes.Response:
                OnResponse(text);
                break;

            case StreamActionTypes.OnPublish:
            {
                var method = MethodModel.FromJson(text);
                if (method is not null)
                {
                    _remoteActions[method.Name] = method;
                    // Прокси-ActionModel: локальный вызов уходит Request на хаб/сервер
                    // Прокси не блокирует receive-loop: ожидание ответа на ThreadPool.
                    if (!ServiceContext.Actions.ContainsKey(method.Name))
                    {
                        ServiceContext.AddAction(method.Name, new ActionModel(method.Name, args =>
                            Task.Run(() => RequestAsync(method.Name, args)).GetAwaiter().GetResult())
                        {
                            Parameters = method.Parameters.ToJson().FromJson<List<Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel.MethodParameterModel>>(),
                            Label = method.Label,
                            Description = method.Description,
                            Type = method.Type,
                            OwnerClientId = method.OwnerClientId
                        });
                    }

                    Info($"on-publish: {method.Name} owner={method.OwnerClientId}");
                    OnPublish?.Invoke(method);
                }
                break;
            }

            case StreamActionTypes.OnUnpublish:
            {
                var method = MethodModel.FromJson(text);
                if (method is not null)
                {
                    _remoteActions.TryRemove(method.Name, out _);
                    ServiceContext.RemoveAction(method.Name);
                    OnUnpublish?.Invoke(method);
                }
                break;
            }

            case StreamActionTypes.Catalog:
                Info($"catalog: {text}");
                break;

            default:
                if (Logging)
                    Info($"<< [{type}] {Truncate(text)}");
                break;
        }
    }

    private async Task OnRequestAsync(string requestText)
    {
        var request = ActionRequest.FromJson(requestText);
        ActionResponse response;
        try
        {
            Info($"exec local {request.ActionName}");
            var result = ServiceContext.DoAction(request.ActionName, request.MessageObject);
            response = new ActionResponse
            {
                ActionSuccess = true,
                SerialKey = request.SerialKey,
                RequestMessage = request,
                ActionResult = result
            };
        }
        catch (Exception ex)
        {
            response = new ActionResponse
            {
                ActionSuccess = false,
                SerialKey = request.SerialKey,
                RequestMessage = request,
                ErrorMessage = ex.Message
            };
        }

        await SendRawAsync(StreamActionTypes.Response, response.ToJson(), encrypt: false);
    }

    private void OnResponse(string responseText)
    {
        var response = ActionResponse.FromJson(responseText);
        var handler = _async.Take(response.SerialKey);
        if (handler is null)
        {
            Info($"response без handler: {Truncate(response.SerialKey)}");
            return;
        }

        if (response.ActionSuccess)
            handler.OnSuccess(response.ActionResult);
        else
            handler.OnError(response.ErrorMessage ?? "Failed");
    }

    private async Task SendRawAsync(
        string type,
        string payload,
        bool encrypt,
        CancellationToken cancellationToken = default)
    {
        if (_call is null || _sessionToken is null)
            throw new InvalidOperationException("Нет активного потока.");

        var bytes = Encoding.UTF8.GetBytes(payload);
        if (encrypt)
        {
            if (_cert is null) throw new InvalidOperationException("Нет сертификата.");
            bytes = PayloadCrypto.Encrypt(bytes, _cert);
        }

        var envelope = new StreamEnvelope
        {
            SessionToken = _sessionToken,
            MessageId = Guid.NewGuid().ToString("N"),
            TimestampUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Type = type,
            Payload = Google.Protobuf.ByteString.CopyFrom(bytes)
        };
        envelope.Headers["client_id"] = _clientId;
        if (encrypt)
            envelope.Headers["encrypted"] = "true";

        await _call.RequestStream.WriteAsync(envelope, cancellationToken);
    }

    private string DecodePayload(StreamEnvelope msg)
    {
        var encrypted = msg.Headers.TryGetValue("encrypted", out var enc) &&
                        string.Equals(enc, "true", StringComparison.OrdinalIgnoreCase);
        if (!encrypted)
            return msg.Payload.ToStringUtf8();

        // ответы сервера в demo обычно без шифрования; если зашифрованы клиентским cert — не расшифруем приватным ключом
        return msg.Payload.ToStringUtf8();
    }

    private void Info(string message)
    {
        if (Logging)
            Console.WriteLine($"[{_clientId}] {message}");
    }

    private static string Truncate(string value) =>
        value.Length <= 120 ? value : value[..120] + "...";
}
