using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

/*
namespace WinDesigner.Console_AppModule.Console_InputModule.EntityFasade
{

    [Label("Тестирование методов управление веб-приложением")]

    public class WebAppUnit
    {
        public IEntityFasade<WebPage> WebPages { get; }

        private WebAppDbContext dbContext = new WebAppDbContext();

        public WebAppUnit()
        {
            WebPages = new DbEntityFasade<WebPage>(dbContext);
        }

        public static void Test()
        {
            var unit = new WebAppUnit();
            unit.WebPages.Create(new WebPage()
            {
                Url = "test"
            });
            unit.WebPages.GetAll().ToJsonOnScreen().WriteToConsole();
        }
    }



    [Label("Страница")]
    public class WebPage: BaseEntity
    {
        [Label("URL")]
        [InputUrl()]
        [NotNullNotEmpty]
        public string Url { get; set; } = "https://localhost:8080/index";

        public WebPage() { }
    }



    [Label("База веб-приложения")]
    public class WebAppDbContext : DbContext
    {
        public DbSet<WebPage> WebPages { get; set; }

        public WebAppDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if(!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer($@"Data Source=DESKTOP-IHJM9RD;Initial Catalog={GetType().GetTypeName()};Integrated Security=True;Connect Timeout=30;Encrypt=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }

        public static void ConfigureWebAppDbContext(IServiceCollection services)
        {
            services.AddDbContext<WebAppDbContext>();
        }
    }




    /// <summary>
    /// 
    /// </summary>
    public class WebAppDbContextInitializer: DbContextInitiliazer<WebAppDbContext>
    {
        public WebAppDbContextInitializer()
        {
        }

        public override Dictionary<string, int> Init(WebAppDbContext context)
        {
            Dictionary<string, int> res = new();
            res[nameof(WebPage)] = this.InitWebPages(context);

            return res;
        }

        private int InitWebPages(WebAppDbContext context)
        {
            if (context.WebPages.Count() == 0)
            {
                context.Add(new WebPage() { Url = "/index" });
                context.Add(new WebPage() { Url = "/about" });
                context.Add(new WebPage() { Url = "/home" });
                context.Add(new WebPage() { Url = "/help" });
                context.Add(new WebPage() { Url = "/login" });
            }
            return context.SaveChanges();
        }
    }
}*/
