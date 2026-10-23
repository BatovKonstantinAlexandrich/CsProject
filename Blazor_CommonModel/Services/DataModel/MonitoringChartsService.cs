using Microsoft.EntityFrameworkCore;
using static Blazor_CommonModel.Services.DataModel.MonitoringDbContext;

namespace Blazor_CommonModel.Services.DataModel
{


    [Label("Формирует модели данных для представлений типа диаграммы.")]
    public class MonitoringChartsService: IControlService<MonitoringView>
    {
        public MonitoringView Control { get; set; }

        public void SetControl(MonitoringView item)
        {
            this.Control = item;
        }



        /// <summary>
        /// Выборка статистических данных, заданной протяженностью.
        /// </summary>
        public ListView SelectBusinessResource(Action<int> onselect)
        {
            using (var db = new MonitoringDbContext())
            {
                return db.CreateSelectList<BusinessResource>(
                    resource => onselect(resource.Id));
            }                
        }


        /// <summary>
        /// Выборка статистических данных, заданной протяженностью.
        /// </summary>
        public LineChartView GetStatisticsInLine(int businessResourceId, DateTime beginDate, int granularity=1, int length=7)
        {
            var dataset = this.GetStatisticsDataset(businessResourceId,beginDate,granularity, length);
            
            var times = dataset.Select(record => record.BeginDate).Select(dt => dt.ToString("dd.MM.yyyy")).Distinct().ToList();
            var indicators = dataset.Select(record => record.Indicator).Select(i => i.Name).Distinct().ToList();
            var line = new LineChartView();
            line.Title = dataset.FirstOrDefault().Resource.Name + " c "+beginDate.ToString("dd.MM.yyyy");
            line.XLabels.Clear();
            line.XLabels.AddRange(times);
            line.Series.Clear();            
            line.Series.AddRange(indicators.Select(indicator => new NamedSeriesArray()
            {
                name = indicator,
                data = dataset.Where(ds => ds.Indicator.Name == indicator).OrderBy(ds => ds.BeginDate).Select(ds => ds.IndicatorValue).ToList()
            }));
            return line;
        }


        /// <summary>
        /// Выборка статистических данных, заданной протяженностью.
        /// </summary>
        public TableView GetStatisticsInTable(int businessResourceId, DateTime beginDate, int granularity = 1, int length = 7)
        {
            var dataset = this.GetStatisticsDataset(businessResourceId, beginDate, granularity, length);

            var times = dataset.Select(record => record.BeginDate).Select(dt => dt.ToString("dd.MM.yyyy")).Distinct().ToList();
            var indicators = dataset.Select(record => record.Indicator).Select(i => i.Name).Distinct().ToList();
            var table = new TableView();
            table.Title = dataset.FirstOrDefault().Resource.Name + " c " + beginDate.ToString("dd.MM.yyyy");
            table.Columns.Clear();
            table.Columns.Add("Показатель");
            table.Columns.AddRange(times);
            indicators.ToList().ForEach(indicator => {
                table.DataRows.Add(indicator);
                table.DataRows.AddRange(dataset.Where(ds => ds.Indicator.Name == indicator).OrderBy(ds => ds.BeginDate).Select(ds => ds.IndicatorValue.ToString()).ToList());
            });
            return table;
        }



        private List<StatisticsData> GetStatisticsDataset(int businessResourceId, DateTime beginDate, int granularity = 1, int length = 7)
        {
            using (var db = new MonitoringDbContext())
            {
                DateTime endDate = granularity == 1 ? beginDate.AddDays(length) :
                    granularity == 2 ? beginDate.AddDays(7 * length) :
                    granularity == 3 ? beginDate.AddMonths(length) :
                    granularity == 4 ? throw new Exception() :
                    granularity == 5 ? beginDate.AddYears(length) :
                    throw new Exception();
                var resultset = db.StatisticsDatas
                        .Include(d => d.Resource)
                        .Include(d => d.Indicator)
                        .Where(data =>
                    (data.BeginDate >= beginDate &&
                    data.BeginDate <= (endDate)) &&
                    data.ResourceId == businessResourceId &&
                    data.Granularity == granularity)
                        .ToList();
                if (resultset.Count() == 0)
                    throw new Exception("Нет данных");
                return resultset;
            }
        }

    }
}
