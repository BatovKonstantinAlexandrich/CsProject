using Console_CommonModel.CommonLib.LibModels.Console_ProgramModule;

namespace Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule;

[Label("{{GetUnitLabel()}}")]
[Description("{{GetUnitDescription()}}")]
internal class BusinessAnaliticsUnit : TestingUnit
{
    public BusinessAnaliticsUnit(IServiceProvider provider) : base(provider)
    {
        Append(new StatisticsModuleBusinessProcessModelTest(provider));
        Append(new StatisticsModuleBusinessDbContextDbSetsTest(provider));
    }

    public string GetUnitLabel()
    {
        return $"Тестирование модели {nameof(BusinessDbContext)}. {typeof(BusinessDbContext).GetLabel()}";
    }

    public string GetUnitDescription()
    {
        return $"{typeof(BusinessDbContext).GetDescription()}";
    }
}
