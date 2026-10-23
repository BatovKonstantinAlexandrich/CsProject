using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

namespace Blazor_CommonModel.Services.DataModel
{

    public class MonitoringDbContext: DeliveryDbContext
    {
        public DbSet<BusinessResource> BusinessResources { get; set; }
        public DbSet<PerformanceIndicator> PerformanceIndicators { get; set; }
        public DbSet<StatisticsData> StatisticsDatas { get; set; }

        public MonitoringDbContext() : base() { }
        public MonitoringDbContext(DbContextOptions<DbContextBase> options) : base(options)
        {


        }

         

        public class PerformanceIndicator: NamedObject
        {
        }

        public class StatisticsData: BaseEntity
        {
            public DateTime BeginDate { get; set; }
            public int ResourceId { get; set; }
            public BusinessResource Resource { get; set; }
            public int IndicatorId { get; set; }
            public PerformanceIndicator Indicator { get; set; }

            [Range(1,5)] //1-dayly,2-weekly,3-monthly,4-season,5-yearly
            public int Granularity { get; set; }

            [NotNullNotEmpty]
            public float IndicatorValue { get; set; }
        }

    }
}
