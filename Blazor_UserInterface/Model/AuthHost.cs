
using Core.Services.CoreAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class AuthHost
{
    public static IServiceCollection AddAuthorizationServices(this IServiceCollection services, IConfiguration Conf )
    {
        Console.WriteLine("ConfigureAuthorizationBackground(...)");
        services.AddScoped<UserMessagesService>();
        services.AddScoped<AuthorizationService>();
        services.AddScoped<APIAuthorization>(provider => provider.GetRequiredService<AuthorizationService>());
        services.AddScoped<APIRegistration>(provider => provider.GetRequiredService<AuthorizationService>());
        services.AddScoped<RegistrationService>(provider => provider.GetRequiredService<AuthorizationService>());
        return services;
    }
}