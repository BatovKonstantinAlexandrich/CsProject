using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;

namespace Console_CommonModel.CommonLib.LibModels.Console_DbContextModule;

[Label($"Тестирование модели {nameof(DbContextBase)}")]
public class DbContextModuleTest : TestingUnit
{
    public DbContextModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new DbContextAdapterWrapsDbContextTest(provider));
    }
}
