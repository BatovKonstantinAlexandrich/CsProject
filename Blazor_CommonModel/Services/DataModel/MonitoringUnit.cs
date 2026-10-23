using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor_CommonModel.Services.DataModel
{

    [Label("Тестирование системы мониторинга")]
    public class MonitoringUnit : TestingUnit
    {
        
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MonitoringDbContext>( options => {
                DbContextBase.OnConfiguring<MonitoringDbContext>(options);
            });
            services.AddTransient<MonitoringUnit>();
            services.AddTransient<MonitoringDbInitiallizer>();
            services.AddTransient<MonitoringChartsService>();            
            services.AddTransient<MonitoringChartsService>();
        }

        public MonitoringUnit(IServiceProvider parent) : base(parent)
        {
            Append(new MonitoringDbInitiallizerTest());
            Append(new MonitoringDbContextTest());
            Append(new MonitoringChartServiceTest());
        }


        [Label("Проверка контекста данных")]
        private class MonitoringDbContextTest : TestingElement
        {
            public override void OnTest()
            {
                this.AssertService<MonitoringDbContext>(db => {

                    db.EnsureHasData();
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                    db.Validate();
                    return true;
                },
                "MonitoringDbContext проверен запросами на выборку",
                "");
            }
        }


        [Label("Тестирование функций выборки статистических данных")]
        private class MonitoringChartServiceTest : TestingElement
        {
            public override void OnTest()
            {
                this.AssertService<MonitoringChartsService>(chartService => {

                    using (var db = new MonitoringDbContext())
                    {
                        foreach(var resource in db.BusinessResources.ToList())
                        {
                            var dataset = chartService.GetStatisticsInLine(resource.Id, 
                                DateTime.Parse("01.06.2026"));
                        }
                    }
                    return true;
                },
                "MonitoringChartsService формирует выборки статических данных для формирования диаграмм",
                "MonitoringChartsService не формирует выборки статических данных");
            }
        }


        [Label("Тестирование класса наполнения первичными данными модель")]
        private class MonitoringDbInitiallizerTest : TestingElement
        {
            public override void OnTest()
            {
                this.AssertService<MonitoringDbInitiallizer>(dbi => {

                    using(var db = new MonitoringDbContext())
                    {
                        db.Database.EnsureDeleted();
                        db.Database.EnsureCreated();

                        var created = dbi.ReInit(db);
                        this.Messages.Add(created.ToJsonOnScreen());

                        return created.Values.All(v => v > 0);
                    }
                },
                "MonitoringDbContext заполнен тестовыми данными",
                "MonitoringDbContext остаётся без данных");
            }
        }


         
    }
}
