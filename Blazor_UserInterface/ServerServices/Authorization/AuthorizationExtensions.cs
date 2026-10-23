


 
 
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;



/// <summary>
/// Подключение служб приложения
/// </summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Добавление сервиса управления сеанссом
    /// </summary>
    /// <param name="services"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationSession(this IServiceCollection services, int seconds)
    {
        Writing.ToConsole($"AddApplicationSession()");

        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(seconds);
            options.Cookie.HttpOnly = false;
            options.Cookie.IsEssential = true;
        });
        
        return services;
    }


    /// <summary>
    /// Использование компонента управления сеансом
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseApplicationSession(this IApplicationBuilder app)
    {
        Writing.ToConsole($"UseApplicationSession()");

        // конфигурация доступа
        app.UseSession();
        return app;
    }
 
    private static void ConfigureEmail(IServiceCollection services)
    {
        services.AddScoped<EmailService>();
    }

    public static IServiceCollection AddSessionService<T>(this IServiceCollection services)
    {
 
        services.AddScoped(typeof(T), (p) => {
            RequestTransient requestScope = p.GetService<RequestTransient>();
            return requestScope.GetSession().Get(typeof(T));
        });
        return services;
    }


    /// <summary>
    /// Конфигурация конвеера обработки запросов
    /// </summary>
    /// <param name="app">конвеер обработки запросов</param>
    /// <param name="configuration">конфигурация приложения</param>
    /// <returns></returns>
    public static IApplicationBuilder UseApplicationAuthorization( this IApplicationBuilder app )
    {
        Writing.ToConsole($"UseApplicationAuthorization()");
            
        // конфигурация доступа                        
        app.UseMiddleware<CanActivateComponent>();
        return app;
    }

        

        
}
