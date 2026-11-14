using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Http;

public class TrackingCircuitHandler : CircuitHandler
{
    private readonly IServiceProvider _sp;
    public ConnectionTracker _tracker;
    private IHttpContextAccessor _httpContextAccessor;

    public TrackingCircuitHandler(IHttpContextAccessor httpContextAccessor, IServiceProvider sp)
    {
        _sp = sp;
        _tracker = sp.Get<ConnectionTracker>();
        _httpContextAccessor = httpContextAccessor;
    }

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        if(_tracker is null )
        {
            _tracker = _sp.Get<ConnectionTracker>();
        }
        if(_httpContextAccessor is null)
        {
            _httpContextAccessor = _sp.Get<IHttpContextAccessor>();
        }
        if (_httpContextAccessor is null)
        {
            return Task.CompletedTask;
        }
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
            if(!_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(nameof(ConnectionInfo)))
            {
                var token = _tracker.Put(new()
                {
                    Ip4 = ip
                });
                _httpContextAccessor.HttpContext.Request.Headers[nameof(ConnectionInfo)] = token;
            }
            else
            {
                var token = _httpContextAccessor.HttpContext.Request.Headers[nameof(ConnectionInfo)];
                var connection = _tracker.Take(token);
            }
            
        }

        return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }

    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        if (_httpContextAccessor is null)
        {
            _httpContextAccessor = _sp.Get<IHttpContextAccessor>();
        }
        if (_httpContextAccessor is null)
        {
            return Task.CompletedTask;
        }
        var token = _httpContextAccessor.HttpContext.Request.Headers[nameof(ConnectionInfo)];

        if (_tracker.ContainsKey(token))
        {
            _tracker.Remove(token);
        }
            
        //_tracker.Remove(circuit.Id);
        return base.OnCircuitClosedAsync(circuit, cancellationToken);
    }
}