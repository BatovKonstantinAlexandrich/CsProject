[Label("Именованный набор данных")]
[Description("Модель данных для графиков и диаграмм, содержащая массив именованных серий данных для визуализации в компонентах LiveCharts")]
public class NamedDataset
{
    [Label("Данные")]
    public List<NamedSeriesArray> series { get; set; } = new();
}
