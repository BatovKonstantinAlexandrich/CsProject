
/// <summary>
/// ! элемент диаграммы
/// </summary>
[Label("Динамика изменений статистической величины")]
public class NamedSeriesArray
{
    [Label("Показатель")]
    [InputOrder(1)]
    public virtual string name { get; set; } = "Название показателя";

    [Label("Данные")]
    [InputPrimitiveCollection()]
    [InputOrder(2)]
    public virtual List<float> data { get; set; } = new() { 1f };
}
