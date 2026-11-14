using System.Text;
using Grpc.Core;
using GrpcPort.Contracts;
using GrpcPort.Contracts.Actions;
using GrpcPort.Server.Actions;
using GrpcPort.Server.Sessions;

namespace GrpcPort.Server.Services;

public sealed class PortServiceImpl : PortService.PortServiceBase
{
    private readonly SessionManager _sessions;
    private readonly ActionBroker _broker;
    private readonly ILogger<PortServiceImpl> _logger;

    public PortServiceImpl(
        SessionManager sessions,
        ActionBroker broker,
        ILogger<PortServiceImpl> logger)
    {
        _sessions = sessions;
        _broker = broker;
        _logger = logger;
    }

    public override Task<AuthResponse> Authenticate(AuthRequest request, ServerCallContext context)
    {
        try
        {
            var session = _sessions.Authenticate(request.ClientId, request.Secret);

            return Task.FromResult(new AuthResponse
            {
                Success = true,
                Message = "OK",
                SessionToken = session.SessionToken,
                EncryptionCertificate = Google.Protobuf.ByteString.CopyFrom(session.EncryptionCertificateDer),
                CertificateThumbprint = session.CertificateThumbprint,
                ExpiresUnix = session.ExpiresUnix
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Task.FromResult(new AuthResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    public override async Task OpenStream(
        IAsyncStreamReader<StreamEnvelope> requestStream,
        IServerStreamWriter<StreamEnvelope> responseStream,
        ServerCallContext context)
    {
        if (!await requestStream.MoveNext(context.CancellationToken))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Ожидалось первое сообщение с session_token."));
        }

        var first = requestStream.Current;
        if (string.IsNullOrWhiteSpace(first.SessionToken) ||
            !_sessions.TryGet(first.SessionToken, out var session) ||
            session is null)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Недействительный session_token."));
        }

        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > session.ExpiresUnix)
        {
            _sessions.Remove(session.SessionToken);
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Сессия истекла."));
        }

        session.MarkConnected();
        _logger.LogInformation("Поток открыт для {ClientId}", session.ClientId);

        await HandleInboundAsync(session, first, context.CancellationToken);

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(
            context.CancellationToken,
            session.LifetimeToken);

        var ct = linked.Token;

        var readerTask = Task.Run(async () =>
        {
            try
            {
                while (await requestStream.MoveNext(ct))
                    await HandleInboundAsync(session, requestStream.Current, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
            }
            finally
            {
                session.Inbound.Writer.TryComplete();
            }
        }, ct);

        var writerTask = Task.Run(async () =>
        {
            try
            {
                await foreach (var envelope in session.Outbound.Reader.ReadAllAsync(ct))
                    await responseStream.WriteAsync(envelope, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
            }
        }, ct);

        var processorTask = ProcessInboundQueueAsync(session, ct);

        try
        {
            await Task.WhenAny(readerTask, writerTask);
        }
        finally
        {
            _broker.OnSessionClosed(session.ClientId);
            session.Close();
            _sessions.Remove(session.SessionToken);
            try { await Task.WhenAll(readerTask, writerTask, processorTask); }
            catch { /* поток закрыт */ }

            _logger.LogInformation("Поток закрыт для {ClientId}", session.ClientId);
        }
    }

    private async Task HandleInboundAsync(
        ClientSession session,
        StreamEnvelope envelope,
        CancellationToken cancellationToken)
    {
        // Служебный ping можно обработать сразу, остальное — через inbound-очередь.
        if (string.Equals(envelope.Type, StreamActionTypes.Ping, StringComparison.OrdinalIgnoreCase))
        {
            await session.Outbound.Writer.WriteAsync(new StreamEnvelope
            {
                SessionToken = session.SessionToken,
                MessageId = Guid.NewGuid().ToString("N"),
                CorrelationId = envelope.MessageId,
                TimestampUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Type = StreamActionTypes.Pong,
                Payload = Google.Protobuf.ByteString.CopyFromUtf8("pong")
            }, cancellationToken);
            return;
        }

        await session.Inbound.Writer.WriteAsync(envelope, cancellationToken);
    }

    private async Task ProcessInboundQueueAsync(ClientSession session, CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var message in session.Inbound.Reader.ReadAllAsync(cancellationToken))
            {
                var encrypted = message.Headers.TryGetValue("encrypted", out var enc) &&
                                string.Equals(enc, "true", StringComparison.OrdinalIgnoreCase);

                string text;
                try
                {
                    text = encrypted
                        ? Encoding.UTF8.GetString(PayloadCrypto.Decrypt(message.Payload.ToByteArray(), session.ServerPfx))
                        : message.Payload.ToStringUtf8();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Не удалось расшифровать/прочитать сообщение {Id}", message.MessageId);
                    continue;
                }

                try
                {
                    await _broker.HandleInboundAsync(session, message, text, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка обработки {Type} от {Client}", message.Type, session.ClientId);
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }
}
