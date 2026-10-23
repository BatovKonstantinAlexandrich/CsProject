using Microsoft.EntityFrameworkCore;


namespace ClassModelLib.ActionsClient
{

    /// <summary>
    /// ActionProgram при выполнении Load, считывает все ActionServiceModels 
    /// загружает сборки, находит тип данных и подключает сервис.
    /// </summary>
    public class ActionDbContext: DbContext
    {
        public DbSet<ActionServiceModel> ActionServiceModels { get; set; }


        public ActionDbContext() { }
        public ActionDbContext(DbContextOptions<ActionDbContext> options) : base(options) { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseInMemoryDatabase(GetType().Name);
                optionsBuilder.UseSqlServer($@"Data Source=DESKTOP-IHJM9RD;Initial Catalog={GetType().Name};Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            }
        }


        /// <summary>
        /// Данные необходимые для создания сервиса
        /// </summary>
        public class ActionServiceModel
        {
            public int Id { get; set; }
            public byte[] AssemblyDll { get; set; }
            public string TypeName { get; set; }
        }
    }

}
