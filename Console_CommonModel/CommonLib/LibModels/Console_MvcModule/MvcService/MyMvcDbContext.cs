using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;

namespace Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService
{
    [Label("База контроллеров API")]
    public class MyMvcDbContext : DbContextBase
    {
        public DbSet<MyApplicationModel> ApplicationModels { get; set; }
        public DbSet<MyControllerModel> ControllerModels { get; set; }
        public DbSet<MyActionModel> ActionModels { get; set; }
        public DbSet<MyParameterDeclarationModel> ParameterModels { get; set; }
    }
}
