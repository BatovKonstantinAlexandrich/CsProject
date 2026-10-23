namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.Console_FileModule;

[Label($"Модуль тестирования модели данных FileDbContext")]
public class FileModuleTest : TestingUnit
{
    public FileModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new FileModuleFileCatalogDefaultRootTest(provider));
        Append(new FileModuleCreateLoadUpdateFilesTest(provider));
    }

    public string GetLabel()
    {
        return $"Тестирование модели {nameof(FileDbContext)}. {typeof(FileDbContext).GetLabel()}";
    }

    public string GetDescription()
    {
        return $"{typeof(FileDbContext).GetDescription()}";
    }
}
