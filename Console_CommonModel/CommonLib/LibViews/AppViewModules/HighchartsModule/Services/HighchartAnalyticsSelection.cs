namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

/// <summary>Параметры выборки показателей из <c>BusinessData</c>.</summary>
[Label("Параметры выборки")]
public sealed class HighchartAnalyticsSelection
{
    [Label("Набор данных")]
    [InputDictionary("BusinessDataset,Name")]
    public int? BusinessDatasetId { get; set; }

    [Label("Периодичность")]
    //[InputDictionary("BusinessGranularity,Name")]
    public int? GranularityId { get; set; }

    [Label("Бизнес-ресурс")]
    //[InputDictionary("BusinessResource,Name")]
    public int? BusinessResourceId { get; set; }

    [Label("Бизнес-показатели")]
    public List<int> BusinessIndicatorIds { get; set; } = new();

    [Label("Дата отчёта")]
    public string BeginDate { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");

    [NotInput]
    public bool HasResourceAndIndicators =>
        BusinessResourceId is > 0 && BusinessIndicatorIds is { Count: > 0 };
    [NotInput]
    public bool HasDatasetFilter =>
        BusinessDatasetId is > 0 && GranularityId is > 0 && !string.IsNullOrWhiteSpace(BeginDate);
}
