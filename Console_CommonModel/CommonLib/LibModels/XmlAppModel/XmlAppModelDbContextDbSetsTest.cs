namespace Console_CommonModel.CommonLib.LibModels.XmlAppModel;

[Label("XmlAppModel: DbSet XmlAppStartups в XmlAppDbContext")]
[Description("XmlAppDbContext объявляет DbSet XmlAppStartups для записей автозапуска.")]
public class XmlAppModelDbContextDbSetsTest : TestingElement
{
    public XmlAppModelDbContextDbSetsTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        Assert(_ =>
        {
            this.LogDbSetSummary(typeof(XmlAppDbContext), "XmlAppDbContext");
            return LibModelsTestHelpers.CountDbSetProperties(typeof(XmlAppDbContext)) >= 1;
        },
            "XmlAppDbContext содержит DbSet для XmlAppStartup",
            "XmlAppDbContext не содержит DbSet XmlAppStartups");
    }
}
