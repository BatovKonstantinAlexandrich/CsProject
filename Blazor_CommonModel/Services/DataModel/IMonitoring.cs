
namespace Blazor_CommonModel.Services.DataModel
{
    public interface IMonitoring: IMonitoringView
    {
        public Dictionary<string, int> InitMonitoringData();
        public Action InitMonitoringDesign();
        public void UpdateMonitoringView();
        public void TestMonitoringModule();
    }


    public interface IMonitoringView
    {
        DateTime BeginDate { get; set; }
        int BusinessResourceId { get; set; }
        int Granularity { get; set; }
        int Length { get; set; }
    }
}