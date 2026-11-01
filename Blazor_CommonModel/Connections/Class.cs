using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Http;

using System.Collections.Concurrent;
public class ConnectionTracker
{
    // Используем ConcurrentDictionary для потокобезопасности
    private readonly ConcurrentDictionary<string, ClientInfo> _connections = new();

    public void AddOrUpdate(string circuitId, string ipAddress, DateTime connectedAt)
    {
        _connections[circuitId] = new ClientInfo
        {
            CircuitId = circuitId,
            IpAddress = ipAddress,
            ConnectedAt = connectedAt,
            LastSeen = DateTime.UtcNow
        };
    }

    public void Remove(string circuitId) => _connections.TryRemove(circuitId, out _);

    public IReadOnlyCollection<ClientInfo> GetAll() => _connections.Values.ToList();
}

public class ClientInfo
{
    public string CircuitId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime ConnectedAt { get; set; }
    public DateTime LastSeen { get; set; }
}
public class TrackingCircuitHandler : CircuitHandler
{
    private readonly ConnectionTracker _tracker;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TrackingCircuitHandler(ConnectionTracker tracker, IHttpContextAccessor httpContextAccessor)
    {
        _tracker = tracker;
        _httpContextAccessor = httpContextAccessor;
    }

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        // Получаем IP из HttpContext в момент открытия
        var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        // Если за прокси, лучше проверить заголовок X-Forwarded-For, 
        // но UseForwardedHeaders уже должен был подменить RemoteIpAddress.
        // На всякий случай можно добавить fallback:
        if (string.IsNullOrEmpty(ip))
        {
            var forwarded = _httpContextAccessor.HttpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            ip = forwarded?.Split(',').FirstOrDefault()?.Trim() ?? "unknown";
        }

        if (!string.IsNullOrEmpty(ip))
        {
            _tracker.AddOrUpdate(circuit.Id, ip, DateTime.UtcNow);
        }

        return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }

    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _tracker.Remove(circuit.Id);
        return base.OnCircuitClosedAsync(circuit, cancellationToken);
    }
}