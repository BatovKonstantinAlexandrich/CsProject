using BlazorErp.Shared.Views.Abstractions;
using BlazorErp.Shared.Views.Models;
using BlazorErp.Shared.Views.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorErp.Shared.DependencyInjection;

public static class SharedViewServiceCollectionExtensions
{
    /// <summary>Регистрирует инфраструктуру динамических представлений (реестр, резолверы).</summary>
    public static IServiceCollection AddBlazorErpSharedViews(this IServiceCollection services)
    {
        services.AddSingleton<IViewRegistry, ViewRegistry>();
        services.AddSingleton<IViewLinkResolver, ViewLinkResolver>();
        services.AddScoped<IViewDataProviderResolver, ViewDataProviderResolver>();
        return services;
    }

    /// <summary>Регистрирует провайдер данных для представления.</summary>
    public static IServiceCollection AddViewDataProvider<TProvider>(this IServiceCollection services)
        where TProvider : class, IViewDataProvider
    {
        services.AddScoped<IViewDataProvider, TProvider>();
        services.AddScoped<TProvider>();
        return services;
    }

    /// <summary>Регистрирует описание представления в singleton-реестре при старте приложения.</summary>
    public static IServiceCollection AddViewDescriptor(this IServiceCollection services, ViewDescriptor descriptor)
    {
        services.AddSingleton(sp =>
        {
            var registry = sp.GetRequiredService<IViewRegistry>();
            registry.Register(descriptor);
            return descriptor;
        });
        return services;
    }
}
