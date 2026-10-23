using Console_CommonModel.CommonLib.LibModels.Console_LocationModule;
using Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService;
using Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcUnit;

namespace Console_CommonModel.CommonLib.LibModels.Console_MvcModule;

[Label("{{GetUnitLabel()}}")]
[Description("{{GetUnitDescription()}}")]
public class MvcModuleTest : TestingUnit
{
    public MvcModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new MvcModuleServiceModelCreationTest(provider));
        Append(new MvcModuleDbContextDbSetsTest(provider));
    }

    public string GetUnitLabel()
    {
        return $"Тестирование модели {nameof(MyMvcDbContext)}. {typeof(MyMvcDbContext).GetLabel()}";
    }

    public string GetUnitDescription()
    {
        return $"{typeof(MyMvcDbContext).GetDescription()}";
    }
}
