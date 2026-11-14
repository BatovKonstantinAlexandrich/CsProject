using Blazor_MyApp.Services.ContextServices;
using Microsoft.AspNetCore.Components.Server.Circuits;


/// <summary>
/// Отслеживает события подключения к Blazor-хабу
/// </summary>
public class ConnectionsHandler : CircuitHandler
{
    private readonly IServiceProvider _sp;
    private IHttpContextAccessor _httpContextAccessor;

    public ConnectionsHandler(IHttpContextAccessor httpContextAccessor, IServiceProvider sp)
    {
        _sp = sp;
        _httpContextAccessor = httpContextAccessor;
    }

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _sp.Get<ILogger<ConnectionsHandler>>().LogInformation("opened");
        lock (circuit)
        {
            // Получаем IP из HttpContext в момент открытия    
            
            if (!_sp.Get<ConnectionsCollection>().ContainsKey(_sp.Get<CookieManager>().Get()))
            {
                ClientInfo client = new()
                {
                    LastActiveTime = DateTime.Now,
                    Ip4 = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = _httpContextAccessor.HttpContext?.Request.Headers.UserAgent
                };
                _sp.Get<CookieManager>().Set(client.SecretKey = _sp.Get<ConnectionsCollection>().Put(client)).Wait();
            }
        }

        return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }

    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        _sp.Get<ConnectionsCollection>().Remove(_sp.Get<CookieManager>().Get());
        _sp.Get<ILogger<ConnectionsHandler>>().LogInformation("closed");
        //_tracker.Remove(circuit.Id);
        return base.OnCircuitClosedAsync(circuit, cancellationToken);
    }
}