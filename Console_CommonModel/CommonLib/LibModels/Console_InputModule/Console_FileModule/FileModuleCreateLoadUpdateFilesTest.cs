using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;

namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.Console_FileModule
{
    [Label($"Тестирования интеграции FileDbContext с файловой системой")]
    public class FileModuleCreateLoadUpdateFilesTest : TestingElement
    {
        public FileModuleCreateLoadUpdateFilesTest(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {


            Assert(p =>
            {
                using (var models = new ModelsDbContext())
                using (var service = new FileDbContext())
                {

                    models.Database.EnsureDeleted();
                    models.Database.EnsureCreated();
                    service.Database.EnsureDeleted();
                    service.Database.EnsureCreated();
                    service.InitCatalogs();

                    var cid = service.CreateCatalogs("Catalogs");
                }
                return true;
            },
            "Метод FileDbContext.CreateCatalogs(string) интегрирует каталоги",
            "Метод FileDbContext.CreateCatalogs(string) не работает");
        }
    }
}