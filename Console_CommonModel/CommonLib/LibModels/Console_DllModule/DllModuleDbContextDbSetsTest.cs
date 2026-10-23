namespace Console_CommonModel.CommonLib.LibModels.Console_DllModule;

[Label("DllModule: DbSet для сборок в DllDbContext")]
[Description("DllDbContext объявляет DbSet DllAssemblies для каталога DLL.")]
public class DllModuleDbContextDbSetsTest : TestingElement
{
    public DllModuleDbContextDbSetsTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        Assert(_ =>
        {
            this.LogDbSetSummary(typeof(DllDbContext), "DllDbContext");
            return LibModelsTestHelpers.CountDbSetProperties(typeof(DllDbContext)) >= 1;
        },
            "DllDbContext объявляет DbSet для хранения DLL",
            "DllDbContext не содержит DbSet");
    }
}
