using Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService;

namespace Console_CommonModel.CommonLib.LibModels.Console_ProgramModule;

[Label("{{GetUnitLabel()}}")]
[Description("{{GetUnitDescription()}}")]
internal class ProgramModuleTest : TestingUnit
{
    public ProgramModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new ProgramModuleProgramFileResourceLinkTest(provider));
    }

    public string GetUnitLabel()
    {
        return $"Тестирование модели {nameof(ProgramDbContext)}. {typeof(ProgramDbContext).GetLabel()}";
    }

    public string GetUnitDescription()
    {
        return $"{typeof(ProgramDbContext).GetDescription()}";
    }
}
