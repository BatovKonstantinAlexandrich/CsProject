 
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
 
using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Поток управления жизненым циклом обьектов сеанса.
/// </summary>
public class AuthorizationBackgroundService : IHostedService, IDisposable
{
    public static int Counter = StartupApplication.AddBackground(ConfigureAuthorizationBackground);
    public static void ConfigureAuthorizationBackground(HostBuilderContext context, IServiceCollection services)
    {
        Console.WriteLine("ConfigureAuthorizationBackground(...)");
        services.AddSingleton<APIUsers, AuthorizationUsers>();
        services.AddSingleton<APIServices, AuthorizationServices>();
        services.AddHostedService<AuthorizationBackgroundService>();
        services.AddSingleton(typeof(AuthorizationOptions), (p) => {
            var section = context.Configuration.GetSection(nameof(AuthorizationOptions));
            
            var result = section.Get<AuthorizationOptions>();
            if (result == null)
            {
                throw new Exception("Дополните конфигурацию приложения appsettings.json\n" + new AuthorizationOptions().ToJsonOnScreen());
            }
            return result;
        });
    }

    private System.Timers.Timer _timer;
    private readonly ILogger<AuthorizationBackgroundService> _logger;
    private readonly AuthorizationOptions _options;
    private readonly APIUsers _users;
    private readonly APIServices _services;


    /// <param name="options"> Параметры </param>
    /// <param name="users"> Коллекция авторизованных пользователей </param>
    /// <param name="services"> Коллекция авторизованных служб </param>
    public AuthorizationBackgroundService(
            ILogger<AuthorizationBackgroundService> logger,
            AuthorizationOptions options,
            APIUsers users,
            APIServices services)
    {
        _logger = logger;
        _options = options;
        _users = users;
        _services = services;
    }
 
    
    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new System.Timers.Timer(_options.CheckTimeout);
        _timer.Elapsed += (Object source, ElapsedEventArgs e) => {            
            _users.DoCheck();
            _services.DoCheck();
        };
        _timer.AutoReset = true;
        _timer.Enabled = true;

        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Stop();
        return Task.CompletedTask;
    }
}


       