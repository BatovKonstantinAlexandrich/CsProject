namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule;

[Label("{{GetUnitLabel()}}")]
[Description("{{GetUnitDescription()}}")]
public class DeliveryModuleTest : TestingUnit
{
    public DeliveryModuleTest() : this(AppProviderService.GetInstance())
    {

    }
    public DeliveryModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new DeliveryDbContextEntityTypesDiscoveryTest(provider));
    }

    public string GetUnitLabel()
    {
        return $"Тестирование модели {nameof(DeliveryDbContext)}. {typeof(DeliveryDbContext).GetLabel()}";
    }

    public string GetUnitDescription()
    {
        return $"{typeof(DeliveryDbContext).GetDescription()}";
    }
}
