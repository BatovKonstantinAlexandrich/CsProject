using System.Threading.Channels;
using GrpcPort.Contracts;

namespace GrpcPort.Server.Sessions;

/// <summary>
/// Сессия подключённого клиента: входящая и исходящая очереди сообщений.
/// </summary>
public sealed class ClientSession : IAsyncDisposable
{
    private readonly CancellationTokenSource _lifetime = new();

    public ClientSession(
        string clientId,
        string sessionToken,
        byte[] encryptionCertificateDer,
        string certificateThumbprint,
        byte[] serverPfx,
        long expiresUnix,
        int capacity = 1024)
    {
        ClientId = clientId;
        SessionToken = sessionToken;
        EncryptionCertificateDer = encryptionCertificateDer;
        CertificateThumbprint = certificateThumbprint;
        ServerPfx = serverPfx;
        ExpiresUnix = expiresUnix;
        CreatedAt = DateTimeOffset.UtcNow;

        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        };

        Inbound = Channel.CreateBounded<StreamEnvelope>(options);
        Outbound = Channel.CreateBounded<StreamEnvelope>(options);
    }

    public string ClientId { get; }
    public string SessionToken { get; }
    public byte[] EncryptionCertificateDer { get; }
    public string CertificateThumbprint { get; }
    public byte[] ServerPfx { get; }
    public long ExpiresUnix { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? ConnectedAt { get; set; }
    public bool IsStreamOpen { get; set; }

    /// <summary>Имена actions, опубликованных этим клиентом.</summary>
    public HashSet<string> PublishedActions { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Очередь сообщений от клиента (микросервисы читают отсюда).</summary>
    public Channel<StreamEnvelope> Inbound { get; }

    /// <summary>Очередь сообщений к клиенту (микросервисы пишут сюда).</summary>
    public Channel<StreamEnvelope> Outbound { get; }

    public CancellationToken LifetimeToken => _lifetime.Token;

    public void MarkConnected()
    {
        ConnectedAt = DateTimeOffset.UtcNow;
        IsStreamOpen = true;
    }

    public void Close()
    {
        if (_lifetime.IsCancellationRequested)
            return;

        IsStreamOpen = false;
        Inbound.Writer.TryComplete();
        Outbound.Writer.TryComplete();
        _lifetime.Cancel();
    }

    public ValueTask DisposeAsync()
    {
        Close();
        _lifetime.Dispose();
        return ValueTask.CompletedTask;
    }
}
