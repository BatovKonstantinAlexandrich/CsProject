using Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Модель аналитической системы
/// </summary>
public static class BusinessAnaliticsModule
{

    /// <summary>
    /// Регистрация зависимостей
    /// </summary>
    public static void ConfigureServices( IServiceCollection services, IConfiguration configuration )
    {
        services.AddDbContext<BusinessDbContext>(BusinessDbContext.Configure);
        services.AddTransient<BusinessAnaliticsService>();
        services.AddTransient<IHighchartBusinessDataService, HighchartBusinessDataService>();
        services.AddTransient<IHighchartViewDataService, HighchartViewDataService>();
    }
}
