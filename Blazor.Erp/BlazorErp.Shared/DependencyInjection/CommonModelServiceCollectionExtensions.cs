using BlazorErp.Shared.CommonModel.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorErp.Shared.DependencyInjection;

public static class CommonModelServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorErpCommonModel(this IServiceCollection services)
    {
        services.AddScoped<IComponentsFactory, BlazorComponentsFactory>();
        return services;
    }
}
