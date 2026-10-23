namespace Console_CommonModel.CommonLib.LibModels.Console_LocationModule;


[Label("{{GetUnitLabel()}}")]
[Description("{{GetUnitDescription()}}")]
internal class LocationModuleTest : TestingUnit
{
    public LocationModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new LocationModuleDbContextRoutesDbSetTest(provider));
        Append(new PageServiceTest());
    }

    public string GetUnitLabel()
    {
        return $"Тестирование модели {nameof(LocationDbContext)}. {typeof(LocationDbContext).GetLabel()}";
    }

    public string GetUnitDescription()
    {
        return $"{typeof(LocationDbContext).GetDescription()}";
    }
}
