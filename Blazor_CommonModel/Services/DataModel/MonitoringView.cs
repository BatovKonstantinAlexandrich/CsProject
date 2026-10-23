namespace Blazor_CommonModel.Services.DataModel
{
    public class MonitoringView : AppViewBase
    {
        public int BusinessResourceId { get; set; }
        public DateTime BeginDate { get; set; }
        public int Granularity { get; set; }
        public int Length { get; set; }

        public MonitoringView()
        {
            this.Name = "MonitoringControl";
            ViewDesigner = new MonitoringChartDesigner();
            this.Append(CreateText(GetType().GetTypeName()));
        }
    }
}
