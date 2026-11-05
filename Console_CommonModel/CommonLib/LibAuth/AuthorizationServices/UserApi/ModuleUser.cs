using System;
using System.Linq;


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class ModuleUser
{
    public static void ConfigureServices(IConfiguration configure, IServiceCollection services)
    {
        //InputConsole.GetLogger<ModuleUser>().LogInformation("Регистрация служб ... ");
        //services.AddHttpContextAccessor();
        services.AddDbContext<DeliveryDbContext>(ConfigureDbContext);
        if (services.Any(descr => descr.ServiceType == typeof(AuthorizationOptions)) == false)
            services.AddSingleton<AuthorizationOptions>();
        //if (services.Any(descr => descr.ServiceType == typeof(MailRuService2)) == false)
        //    services.AddTransient<MailRuService2>();
        
        services.AddScoped<ITokenProvider, ClientLocalStorageTokenProvider>();
        
        
        
        services.AddScoped<SigninUser<DeliveryDbContext>>();
        services.AddScoped<SignupUser<DeliveryDbContext>>();
        services.AddScoped<UserGroupsService<DeliveryDbContext>>();
        //services.AddScoped<IJSInvoke, JSInvoke>();
        //services.AddScoped<ILocalStorage, LocalStorage>();
        services.AddScoped<ITokenProvider, MemoryTokenProvider>();
        services.AddSingleton<AuthorizationUsers>();
        services.AddSingleton(typeof(APIActiveCollection<UserContext>),sp=>sp.GetRequiredService<AuthorizationUsers>());
        services.AddSingleton(typeof(APIUsers),sp=>sp.GetRequiredService<AuthorizationUsers>());
        services.AddScoped<GroupingUser<DeliveryDbContext>>();
        //services.AddScoped<SignupUser>();
        //services.AddScoped<SigninUser<DeliveryDbContext>>();
        services.AddScoped<ManageUser<DeliveryDbContext>>();
        //InputConsole.GetLogger<ModuleUser>().LogInformation("Регистрация служб ... Успешно завершена");

    }

    public static void ConfigureServices<TDbContext, TTokenProvider>(IConfiguration configure, IServiceCollection services)
        where TDbContext : DeliveryDbContext
        where TTokenProvider : class, ITokenProvider
    {
        //InputConsole.GetLogger<ModuleUser>().LogInformation("Регистрация служб ... ");
        //services.AddHttpContextAccessor();
        services.AddDbContext<TDbContext>(ConfigureDbContext);
        if (services.Any(descr => descr.ServiceType == typeof(AuthorizationOptions)) == false)
            services.AddSingleton<AuthorizationOptions>();
        //if (services.Any(descr => descr.ServiceType == typeof(MailRuService2)) == false)
        //    services.AddTransient<MailRuService2>();

        services.AddScoped<UserGroupsService<TDbContext>>();
        //services.AddScoped<IJSInvoke, JSInvoke>();
        //services.AddScoped<ILocalStorage, LocalStorage>();
        services.AddScoped<ITokenProvider, TTokenProvider>();
        services.AddSingleton<AuthorizationUsers>();
        services.AddSingleton(typeof(APIActiveCollection<UserContext>), sp => sp.GetRequiredService<AuthorizationUsers>());
        services.AddSingleton(typeof(APIUsers), sp => sp.GetRequiredService<AuthorizationUsers>());
        services.AddScoped<GroupingUser<DeliveryDbContext>>();
        //services.AddScoped<SignupUser>();
        services.AddScoped<SigninUser<TDbContext>>();
        services.AddScoped<ManageUser<DeliveryDbContext>>();
        //InputConsole.GetLogger<ModuleUser>().LogInformation("Регистрация служб ... Успешно завершена");

    }

    private static void ConfigureDbContext(DbContextOptionsBuilder builder)
    {
        builder.UseSqlServer($@"Data Source=DESKTOP-IHJM9RD;Initial Catalog={nameof(DbContextUser)};Integrated Security=True;Connect Timeout=30;Encrypt=False;Application Intent=ReadWrite;Multi Subnet Failover=False" );
    }
}