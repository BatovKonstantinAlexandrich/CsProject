using Console_CommonModel.CommonLib.LibModels;

namespace Console_CommonModel.CommonLib.LibModels.Console_DbContextModule;

[Label("DbContextModule: DbContextAdapter оборачивает DbContext")]
[Description("DbContextAdapter создаётся для экземпляра DbContext без ошибок инициализации.")]
public class DbContextAdapterWrapsDbContextTest : TestingElement
{
    public DbContextAdapterWrapsDbContextTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        LogMessage("ServiceBuilderDbContext + EnsureStorageCreated + DbContextAdapter");
        Assert(_ =>
        {
            using var db = new ServiceBuilderDbContext();
            db.EnsureStorageCreated();
            LogMessage("Хранилище ServiceBuilderDbContext готово");
            var adapter = new DbContextAdapter(db);
            LogMessage($"DbContextAdapter создан: {adapter != null}");
            return adapter != null;
        },
        "DbContextAdapter создаётся для контекста данных",
        "DbContextAdapter не создаётся");
    }
}
