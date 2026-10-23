namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

/// <summary>Выбор биндера данных для типа диаграммы (по имени представления / модели).</summary>
public static class HighchartChartDataBinderResolver
{
    private static readonly IHighchartChartDataBinder[] Binders =
    {
        new HighchartPieChartDataBinder(),
        new HighchartScatterChartDataBinder(),
        new HighchartCartesianChartDataBinder(),
    };

    public static IHighchartChartDataBinder Resolve(string chartTypeName)
    {
        foreach (var binder in Binders)
        {
            if (binder.CanBind(chartTypeName))
                return binder;
        }

        return Binders[^1];
    }

    public static IReadOnlyList<IHighchartChartDataBinder> All => Binders;
}
