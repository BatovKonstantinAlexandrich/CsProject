namespace Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule;

[Label("StatisticsModule: создание BusinessProcess")]
[Description("Модель BusinessProcess создаётся с непустой коллекцией BusinessFunctions.")]
public class StatisticsModuleBusinessProcessModelTest : TestingElement
{
    public StatisticsModuleBusinessProcessModelTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        LogMessage("BusinessProcess с Name=Checkout");
        Assert(_ =>
        {
            var process = new BusinessProcess { Name = "Checkout" };
            LogMessage($"BusinessProcess.Name = {process.Name}, BusinessFunctions = {(process.BusinessFunctions != null ? "OK" : "null")}");
            return process.Name == "Checkout" && process.BusinessFunctions != null;
        },
        "BusinessProcess создаётся с коллекцией BusinessFunctions",
        "BusinessProcess не инициализируется");
    }
}

[Label("StatisticsModule: DbSet в BusinessDbContext")]
[Description("BusinessDbContext объявляет хотя бы один DbSet для сущностей аналитики.")]
public class StatisticsModuleBusinessDbContextDbSetsTest : TestingElement
{
    public StatisticsModuleBusinessDbContextDbSetsTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        Assert(_ =>
        {
            this.LogDbSetSummary(typeof(BusinessDbContext), "BusinessDbContext");
            return LibModelsTestHelpers.CountDbSetProperties(typeof(BusinessDbContext)) >= 1;
        },
            "BusinessDbContext содержит DbSet для бизнес-моделей",
            "BusinessDbContext не содержит DbSet");
    }
}
