namespace Console_CommonModel.CommonLib.LibModels.Console_ServiceModule;

[Label("Тестирование контекста данных ServiceDbContext")]
public class ServiceModuleTest : TestingUnit
{
    public ServiceModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new ServiceModuleDataIntegrationStructureTest(provider));
    }
}
