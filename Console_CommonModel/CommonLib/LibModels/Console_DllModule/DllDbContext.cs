using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;

namespace Console_CommonModel.CommonLib.LibModels.Console_DllModule
{

    /// <summary>Контекст для хранения сгенерированных DLL.</summary>
    public class DllDbContext : DbContextBase
    {
        public virtual DbSet<DllAssemblyRecord> DllAssemblies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    $@"Data Source=DESKTOP-IHJM9RD;Initial Catalog={GetType().Name};Integrated Security=True;Connect Timeout=30;Encrypt=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            }
        }
    }
}
