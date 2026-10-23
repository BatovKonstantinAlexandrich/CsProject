

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



/// <summary>
/// Расширение для регистрации сервисов Email
/// </summary>
public static class EmailExtensions
{

    /// <summary>
    /// Расширение для регистрации сервисов Email
    /// </summary>
    public static IServiceCollection AddEmail(this IServiceCollection services)
    {
        services.AddSingleton<EmailOptions>();
        services.AddScoped<IEmailService, EmailService>();            
        return services;
    }
}
