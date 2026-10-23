namespace Console_CommonModel.CommonLib.LibModels.Console_ServiceModule;

[Label("ServiceModule: DataIntegration — абстракция интеграции данных")]
[Description("Типы DataIntegration и связанные контракты доступны для подключения внешних источников.")]
public class ServiceModuleDataIntegrationStructureTest : TestingElement
{
    public ServiceModuleDataIntegrationStructureTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        LogMessage("Рефлексия типа DataIntegration<,,>");
        Assert(_ =>
        {
            var integrationType = typeof(DataIntegration<,,>);
            LogMessage($"IsAbstract={integrationType.IsAbstract}, IsGenericTypeDefinition={integrationType.IsGenericTypeDefinition}");
            return integrationType.IsAbstract && integrationType.IsGenericTypeDefinition;
        },
        "DataIntegration объявлен как абстрактный обобщённый сервис",
        "DataIntegration имеет некорректную структуру");
    }
}
