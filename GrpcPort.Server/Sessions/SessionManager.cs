using System.Collections.Concurrent;
using GrpcPort.Contracts;
using GrpcPort.Server.Services;

namespace GrpcPort.Server.Sessions;

public sealed class SessionManager
{
    private readonly ConcurrentDictionary<string, ClientSession> _byToken = new();
    private readonly ConcurrentDictionary<string, string> _tokenByClient = new();
    private readonly CertificateService _certificates;
    private readonly ILogger<SessionManager> _logger;
    private readonly TimeSpan _sessionLifetime = TimeSpan.FromHours(8);

    public SessionManager(CertificateService certificates, ILogger<SessionManager> logger)
    {
        _certificates = certificates;
        _logger = logger;
    }

    public ClientSession Authenticate(string clientId, string secret)
    {
        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(secret))
            throw new UnauthorizedAccessException("client_id и secret обязательны.");

        // Демо-учётные данные. В проде — проверка по БД / Identity / mTLS.
        if (!IsValidCredential(clientId, secret))
            throw new UnauthorizedAccessException("Неверный client_id или secret.");

        if (_tokenByClient.TryGetValue(clientId, out var oldToken) &&
            _byToken.TryRemove(oldToken, out var oldSession))
        {
            oldSession.Close();
            _logger.LogInformation("Перевыпуск сессии для {ClientId}", clientId);
        }

        var issued = _certificates.IssueForClient(clientId, _sessionLifetime);
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var session = new ClientSession(
            clientId,
            token,
            issued.PublicCertificateDer,
            issued.Thumbprint,
            issued.ServerPfx,
            issued.ExpiresUnix);

        _byToken[token] = session;
        _tokenByClient[clientId] = token;

        _logger.LogInformation(
            "Клиент {ClientId} авторизован. Thumbprint={Thumbprint}, Expires={Expires}",
            clientId,
            issued.Thumbprint,
            DateTimeOffset.FromUnixTimeSeconds(issued.ExpiresUnix));

        return session;
    }

    public bool TryGet(string sessionToken, out ClientSession? session) =>
        _byToken.TryGetValue(sessionToken, out session);

    public bool TryGetByClientId(string clientId, out ClientSession? session)
    {
        session = null;
        if (!_tokenByClient.TryGetValue(clientId, out var token))
            return false;
        return _byToken.TryGetValue(token, out session);
    }

    public IReadOnlyCollection<ClientSession> GetAll() => _byToken.Values.ToArray();

    /// <summary>Публикация сообщения клиенту через исходящую очередь.</summary>
    public async Task<bool> EnqueueOutboundAsync(
        string clientId,
        StreamEnvelope envelope,
        CancellationToken cancellationToken = default)
    {
        if (!_tokenByClient.TryGetValue(clientId, out var token) ||
            !_byToken.TryGetValue(token, out var session) ||
            !session.IsStreamOpen)
        {
            return false;
        }

        await session.Outbound.Writer.WriteAsync(envelope, cancellationToken);
        return true;
    }

    public void Remove(string sessionToken)
    {
        if (_byToken.TryRemove(sessionToken, out var session))
        {
            _tokenByClient.TryRemove(session.ClientId, out _);
            session.Close();
            _logger.LogInformation("Сессия {ClientId} закрыта", session.ClientId);
        }
    }

    private static bool IsValidCredential(string clientId, string secret) =>
        secret == $"secret-{clientId}";
}
