
using Newtonsoft.Json.Linq;


[ViewUsageExample("var view = new ChartView { Text = \"Диаграмма\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<chart-view Text=\"Диаграмма\" Dock=\"Top\" />")]
[Label("Диаграмма")]
[Description("Универсальная диаграмма с выбором типа")]
[Icon("line_chart")]
[NotInput()]
public class ChartView : AppViewBase
{
    [Label("Тип диаграммы")]
    [InputSelect("Bar,Core,Line,Org,Column")]
    public string Type { get => Get<string>(nameof(Type)); set => Set<string>(nameof(Type), value); }

    public ChartView()
    {
       
        Type = "Line";
        ViewDesigner = CreateChartViewDesignerService(this);
    }

    private DesignerBase CreateChartViewDesignerService(ChartView chartView)
    {
        throw new NotImplementedException();
    }

    [OnChange(nameof(Type))]
    public virtual void OnChartTypeChanged()
    {
        RefreshDiagram();
    }

    /// <summary>Пересоздаёт вложенную диаграмму при смене <see cref="Type"/> или данных.</summary>
    public void RefreshDiagram()
    {
        RunOnUiThread(() =>
        {
            //if (control is not ChartControl chartControl)
            //    return;

            //var factory = new ComponentsFactory();
            //var diagram = factory.CreateChart(this);
            //if (diagram is null)
            //    return;

            //diagram.Width = Width;
            //diagram.Height = Height;
            //chartControl.SetDiagram(diagram);
        });
    }

    /// <summary>
    /// Используется для преобразования структуры данных в модель диаграммы
    /// </summary>
    public object MapSeries(JArray dataset, string nameExpression, string dataExpression)
    {
        var result = new List<object>();
        foreach (var p in dataset)
        {
            string name = p[nameExpression].Value<string>();
            List<object> data = new();
            foreach (var datakey in dataExpression.Split(","))
                data.Add(p[datakey].Value<object>());
            result.Add(new { name, data = data.ToArray() });
        }
        return result;
    }
}
