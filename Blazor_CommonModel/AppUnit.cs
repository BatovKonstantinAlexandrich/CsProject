using Blazor_CommonModel.Services.DataModel;

namespace Blazor_CommonModel.App
{
    [Label("Модуль тестирования всего приложения")]
    [Description("")]
    public class AppUnit: TestingUnit
    {
        public AppUnit() : this(AppProviderService.GetInstance()) { }
        public AppUnit(IServiceProvider provider)
        {
            Append(new MonitoringUnit(provider));
        }
    }
}
 