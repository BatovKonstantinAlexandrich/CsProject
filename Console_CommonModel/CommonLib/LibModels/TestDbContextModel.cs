using Console_CommonModel.CommonLib.LibData.Console_DataModule;

namespace Console_CommonModel.CommonLib.LibModels
{

    [Label("Тестирование модели {{GetDbContextTypeName()}}")]
    public class TestDbContextModel<TDbContext>: TestingService<TDbContext>
        where TDbContext: DbContextBase
    {
        public TestDbContextModel(IServiceProvider sp) : base(sp)
        {
        }

        public override bool OnServiceTest(TDbContext service)
        {
            foreach(var entityType in service.GetEntitiesTypes())
            {
                try
                {
                    var repository = service.GetRepositoryForType(entityType);
                    var items = repository.GetAll();
                }
                catch (Exception ex)
                {
                    this.Failed = true;
                    this.Messages.Add($"Не удалось получить записи {entityType.GetTypeName()}");
                }
            }
            return !Failed;
        }

        public string GetDbContextTypeName()
        {
            return typeof(TDbContext).GetTypeName();
        }
    }
}
