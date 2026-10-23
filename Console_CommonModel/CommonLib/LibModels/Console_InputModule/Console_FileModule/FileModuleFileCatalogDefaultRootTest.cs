namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.Console_FileModule;

[Label("FileModule: корневой путь FileCatalog по умолчанию")]
[Description("Конструктор FileCatalog задаёт Name=D:\\ и пустую коллекцию Files.")]
public class FileModuleFileCatalogDefaultRootTest : TestingElement
{
    public FileModuleFileCatalogDefaultRootTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        LogMessage("Создание FileCatalog по умолчанию");
        Assert(_ =>
        {
            var catalog = new FileCatalog();
            LogMessage($"FileCatalog.Name = \"{catalog.Name}\", Files = {(catalog.Files != null ? "инициализирована" : "null")}");
            return catalog.Name == "D:\\" && catalog.Files != null;
        },
        "FileCatalog создаётся с корневым каталогом D:\\",
        "FileCatalog имеет некорректные значения по умолчанию");
    }
}
