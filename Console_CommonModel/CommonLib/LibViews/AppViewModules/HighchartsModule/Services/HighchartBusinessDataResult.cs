namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;

/// <summary>Ряды и подписи оси X, полученные из <c>BusinessData</c>.</summary>
public sealed class HighchartBusinessDataResult
{
    public Dictionary<string, List<float?>> Series { get; init; } = new(StringComparer.Ordinal);
    public List<string> Categories { get; init; } = new();
}
