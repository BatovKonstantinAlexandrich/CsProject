using Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService;

namespace Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcUnit;

[Label("MvcModule: создание MyMvcServiceModel")]
[Description("Экземпляр MyMvcServiceModel создаётся без ошибок конструктора.")]
public class MvcModuleServiceModelCreationTest : TestingElement
{
    public MvcModuleServiceModelCreationTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        LogMessage("Конструктор MyMvcServiceModel");
        Assert(_ =>
        {
            var model = new MyMvcServiceModel();
            LogMessage($"MyMvcServiceModel: {(model != null ? "создан" : "null")}");
            return model != null;
        },
        "MyMvcServiceModel создаётся",
        "MyMvcServiceModel не создаётся");
    }
}
