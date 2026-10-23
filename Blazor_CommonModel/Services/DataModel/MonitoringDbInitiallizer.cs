

namespace Blazor_CommonModel.Services.DataModel
{
    public class MonitoringDbInitiallizer: DbContextInitiliazer<MonitoringDbContext>
    {
        public override Dictionary<string, int> Init(MonitoringDbContext context)
        {
            var r = new Dictionary<string, int>();
            r[nameof(BusinessIndicator)] = this.InitIndicators(context);
            r[nameof(BusinessResource)] = this.InitResource(context);
            r[nameof(BusinessIndicator)] = this.InitStatistics(context);
            return r;
        }

        private int InitStatistics(MonitoringDbContext db)
        {
            var random = new Random();
            // 3. Статистические данные
            var stats = new List<MonitoringDbContext.StatisticsData>();
            var startDate = DateTime.Parse("01.06.2026");
            
            int daysBack = 30;
            int pointsPerDay = 24;
            foreach (var resource in db.BusinessResources.ToList())
            {
                foreach (var indicator in db.PerformanceIndicators.ToList())
                {
                    var ts = startDate;
                    for (int day = 0; day < daysBack; day++)
                    {
                        ts.AddDays(1);
                        var value = random.Next() * 100;
                        stats.Add(new MonitoringDbContext.StatisticsData
                        {
                            ResourceId = resource.Id,
                            IndicatorId = indicator.Id,
                            BeginDate = ts,
                            IndicatorValue = (float)value,
                            Granularity = 1
                        });
                        
                    }
                }
            }

            db.StatisticsDatas.AddRange(stats);
            return db.SaveChanges();
        }
        private int InitResource(MonitoringDbContext context)
        {
            if(context.BusinessResources.Count()==0)
            {
                context.BusinessResources.AddRange(new List<BusinessResource>() {
                    new(){ Name = "Продавец автозапчастей" },
                    new(){ Name = "Автомеханик" },
                    new(){ Name = "Кассир" }
                });                
            }
            return context.SaveChanges();
        }

        private int InitIndicators(MonitoringDbContext context)
        {
            if (context.PerformanceIndicators.Count() == 0)
            {
                context.PerformanceIndicators.AddRange(new List<MonitoringDbContext.PerformanceIndicator>() {
                    new(){ Name = "Продано товаров" },
                    new(){ Name = "Обслужено клиентов" },
                    new(){ Name = "Получено денег" }
                });
            }
            return context.SaveChanges();
        }
    }
}
