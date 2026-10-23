namespace Console_CommonModel.CommonLib.LibModels.XmlAppModel;

[Label("Тестирование сборки приложения из XML")]
public class XmlAppModelModuleTest : TestingUnit
{
    public XmlAppModelModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new XmlAppModelDbContextDbSetsTest(provider));
    }
}
