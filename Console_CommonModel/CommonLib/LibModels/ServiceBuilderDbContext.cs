using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel;


namespace Console_CommonModel.CommonLib.LibModels;


[Description("Сборки подключенных служб")]
public class ServiceBuilderDbContext : DbContextBase
{
    public DbSet<RegisteredWsdlService> RegisteredWsdlServices { get; set; }

    public static string GetDatabasePath()
    {
        var dir = Path.Combine(Directory.GetCurrentDirectory(), "Projects");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, "service_builder.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
            options.UseSqlite($"Data Source={GetDatabasePath()}");
    }

    public void EnsureStorageCreated() => Database.EnsureCreated();
}
