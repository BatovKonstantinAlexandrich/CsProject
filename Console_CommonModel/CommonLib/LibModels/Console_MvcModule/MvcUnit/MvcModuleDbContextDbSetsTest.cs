using Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService;

namespace Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcUnit;

[Label("MvcModule: MyMvcDbContext содержит DbSet приложений")]
[Description("MyMvcDbContext объявляет DbSet для хранения MVC-приложений и связанных сущностей.")]
public class MvcModuleDbContextDbSetsTest : TestingElement
{
    public MvcModuleDbContextDbSetsTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        Assert(_ =>
        {
            this.LogDbSetSummary(typeof(MyMvcDbContext), "MyMvcDbContext");
            return LibModelsTestHelpers.CountDbSetProperties(typeof(MyMvcDbContext)) >= 1;
        },
            "MyMvcDbContext объявляет DbSet для MVC-моделей",
            "MyMvcDbContext не содержит DbSet");
    }
}
