namespace Console_CommonModel.CommonLib.LibModels.Console_DllModule;

[Label("{{GetUnitLabel()}}")]
[Description("{{GetUnitDescription()}}")]
public class DllModuleTest : TestingUnit
{
    public DllModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new DllModuleDbContextDbSetsTest(provider));
    }

    public string GetUnitLabel()
    {
        try
        {
            return $"Тестирование модели {nameof(DllDbContext)}. {typeof(DllDbContext).GetLabel()}";
        }
        catch(Exception ex)
        {
            throw;
        }
    }

    public string GetUnitDescription()
    {
        return $"{typeof(DllDbContext).GetDescription()}";
    }
}
