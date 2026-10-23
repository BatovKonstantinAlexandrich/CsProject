using Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService;

namespace Console_CommonModel.CommonLib.LibModels.Console_WebModule;

[Label("{{GetUnitLabel()}}")]
[Description("{{GetUnitDescription()}}")]
internal class WebModuleTest : TestingUnit
{
    public WebModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new WebModuleOdbcMiddlewareContractTest(provider));
    }

    public string GetUnitLabel()
    {
        return $"Тестирование модели веб-приложения";
    }

    public string GetUnitDescription()
    {
        return $"";
    }

}
