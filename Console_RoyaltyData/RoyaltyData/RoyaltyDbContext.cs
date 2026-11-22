using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Console_RoyaltyData.RoyaltyEntities;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using pickpoint_delivery_service;

namespace Console_RoyaltyData.RoyaltyData
{
    public class RoyaltyDbContext : DeliveryDbContext
    {
        
        public DbSet<Report> Reports { get; set; }
        public DbSet<Invoce> Invoces { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<EmployeeRoles> EmployeeRoles { get; set; }
        public DbSet<RoyaltyCalculation> RoyaltyCalculations { get; set; }
        public DbSet<RoyaltyMonthData> RoyaltyMonthData { get; set; }


        public RoyaltyDbContext(DbContextOptions<DbContextBase> options) : base(options)
        {
        }

        public RoyaltyDbContext(): base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //            this.Info($"Настройка {GetType().GetTypeName()}");

            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.AddInterceptors(RoyaltySchemaInterceptor.Instance);
            if (!optionsBuilder.IsConfigured)
            {
                var filename = $"{GetType().Name}.db";
                if(!System.IO.File.Exists(filename))
                {
                    System.IO.File.WriteAllBytes(filename, new byte[0]);
                }
                optionsBuilder.UseSqlite($"Data Source={filename}");
            }
        }

        public static void OnConfiguring<TRoyaltyDbContext>(DbContextOptionsBuilder optionsBuilder)
        {
       
            if (!optionsBuilder.IsConfigured)
            {
                var filename = $"{typeof(TRoyaltyDbContext).Name}.db";
                if (!System.IO.File.Exists(filename))
                {
                    System.IO.File.WriteAllBytes(filename, new byte[0]);
                }
                optionsBuilder.UseSqlite($"Data Source={filename}");
            }
        }
    }
}
