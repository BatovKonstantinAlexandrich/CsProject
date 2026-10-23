using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;

namespace Console_CommonModel.CommonLib.LibModels.XmlAppModel
{

    [Label("Управление приложениями")]
    public class XmlAppDbContext : DbContextBase
    {
        public virtual DbSet<XmlAppStartup> XmlAppStartups { get; set; }
    }


}
