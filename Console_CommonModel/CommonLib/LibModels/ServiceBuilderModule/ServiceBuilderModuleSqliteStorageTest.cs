namespace Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;

[Label("ServiceBuilderModule: SQLite-хранилище ServiceBuilderDbContext")]
[Description("ServiceBuilderDbContext создаётся с провайдером SQLite для локального каталога сервисов.")]
public class ServiceBuilderModuleSqliteStorageTest : TestingElement
{
    public ServiceBuilderModuleSqliteStorageTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        LogMessage("ServiceBuilderDbContext: EnsureStorageCreated + CanConnect");
        Assert(_ =>
        {
            using var db = new ServiceBuilderDbContext();
            db.EnsureStorageCreated();
            var canConnect = db.Database.CanConnect();
            LogMessage($"Database.CanConnect() = {canConnect}");
            return canConnect;
        },
        "ServiceBuilderDbContext подключается к SQLite",
        "ServiceBuilderDbContext не подключается к SQLite");
    }
}
