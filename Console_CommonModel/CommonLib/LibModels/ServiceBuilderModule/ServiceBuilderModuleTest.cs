using Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService;

namespace Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;

[Label("{{GetUnitLabel()}}")]
[Description("{{GetUnitDescription()}}")]
public class ServiceBuilderModuleTest : TestingUnit
{
    public ServiceBuilderModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new ServiceBuilderModuleSqliteStorageTest(provider));
    }

    public string GetUnitLabel()
    {
        return $"Тестирование модели {nameof(ServiceBuilderDbContext)}. {typeof(ServiceBuilderDbContext).GetLabel()}";
    }

    public string GetUnitDescription()
    {
        return $"{typeof(ServiceBuilderDbContext).GetDescription()}";
    }
}
