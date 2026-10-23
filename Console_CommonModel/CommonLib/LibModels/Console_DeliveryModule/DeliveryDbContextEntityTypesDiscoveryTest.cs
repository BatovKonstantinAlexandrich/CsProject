using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices.DeliveryDataServices;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule;

[Label("DeliveryModule: обнаружение типов сущностей DeliveryDbContext")]
[Description("GetEntitiesTypes возвращает непустой набор типов сущностей контекста доставки.")]
public class DeliveryDbContextEntityTypesDiscoveryTest : TestingElement
{
    public DeliveryDbContextEntityTypesDiscoveryTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        LogMessage("DeliveryDbContext.GetEntitiesTypes()");
        Assert(_ =>
        {
            using var db = new DeliveryDbContext();
            var types = db.GetEntitiesTypes().ToList();
            LogMessage($"Типов сущностей в контексте доставки: {types.Count}");
            return types.Count > 0;
        },
        "DeliveryDbContext возвращает список типов сущностей",
        "DeliveryDbContext не содержит сущностей");
    }
}
