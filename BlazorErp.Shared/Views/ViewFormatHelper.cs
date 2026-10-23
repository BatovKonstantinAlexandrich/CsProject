using BlazorErp.Shared.Views.Models;
using MudBlazor;

namespace BlazorErp.Shared.Views;

internal static class ViewFormatHelper
{
    public static string FormatCell(IReadOnlyDictionary<string, object?> row, ViewColumnDefinition col)
    {
        if (!row.TryGetValue(col.Name, out var value) || value == null)
            return "—";

        return col.Format switch
        {
            "date" when value is DateTime dt => dt.ToString("dd.MM.yyyy"),
            "money" when value is decimal dec => dec.ToString("N2") + " ₽",
            "money" when value is double dbl => dbl.ToString("N2") + " ₽",
            "int" => Convert.ToInt32(value).ToString("N0"),
            _ => value.ToString() ?? "—"
        };
    }

    public static string FormatSum(IList<IReadOnlyDictionary<string, object?>> rows, ViewColumnDefinition col)
    {
        if (!col.ShowSum || rows.Count == 0)
            return "0";

        if (col.Format is "money")
        {
            decimal sum = 0;
            foreach (var row in rows)
            {
                if (row.TryGetValue(col.Name, out var val) && val != null)
                    sum += Convert.ToDecimal(val);
            }

            return sum.ToString("N2") + " ₽";
        }

        if (col.Format is "int")
        {
            var sum = rows.Sum(row =>
                row.TryGetValue(col.Name, out var val) && val != null ? Convert.ToInt32(val) : 0);
            return sum.ToString("N0");
        }

        return string.Empty;
    }

    public static ChartType ToMudChartType(ViewChartType chartType) => chartType switch
    {
        ViewChartType.Line => ChartType.Line,
        ViewChartType.Bar => ChartType.Bar,
        ViewChartType.Pie => ChartType.Pie,
        _ => ChartType.Bar
    };
}
