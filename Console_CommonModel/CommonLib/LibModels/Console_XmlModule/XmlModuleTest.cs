using Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService;

namespace Console_CommonModel.CommonLib.LibModels.Console_XmlModule;

[Label("{{GetUnitLabel()}}")]
[Description("{{GetUnitDescription()}}")]
internal class XmlModuleTest : TestingUnit
{
    public XmlModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new XmlCompilerTest(provider));
        Append(new ComponentFactoryTest(provider));
    }

    public string GetUnitLabel()
    {
        return $"Тестирование {nameof(XmlNodeCompiler)}. {typeof(XmlNodeCompiler).GetLabel()}";
    }

    public string GetUnitDescription()
    {
        return $"{typeof(XmlNodeCompiler).GetDescription()}";
    }
}
