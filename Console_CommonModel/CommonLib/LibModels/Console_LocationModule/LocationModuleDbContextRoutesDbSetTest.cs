namespace Console_CommonModel.CommonLib.LibModels.Console_LocationModule;

[Label("LocationModule: DbSet маршрутов AppRoutes")]
[Description("LocationDbContext содержит DbSet AppRoutes для URI и привязки к страницам.")]
public class LocationModuleDbContextRoutesDbSetTest : TestingElement
{
    public LocationModuleDbContextRoutesDbSetTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        Assert(_ =>
        {
            this.LogDbSetSummary(typeof(LocationDbContext), "LocationDbContext");
            return LibModelsTestHelpers.CountDbSetProperties(typeof(LocationDbContext)) >= 2;
        },
            "LocationDbContext содержит DbSet для страниц и маршрутов",
            "LocationDbContext не содержит ожидаемых DbSet");
    }
}
