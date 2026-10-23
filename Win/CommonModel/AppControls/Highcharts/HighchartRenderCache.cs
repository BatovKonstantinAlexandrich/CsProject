using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

namespace WinFormsApp.AppComponents.AppControls.Highcharts;

/// <summary>Кэш JSON и разобранного <see cref="JObject"/> для повторной отрисовки одинаковых моделей Highcharts.</summary>
public static class HighchartRenderCache
{
    private static readonly ConcurrentDictionary<string, JObject> ParsedByJson = new(StringComparer.Ordinal);

    public static JObject GetParsedRoot(Highchart options)
    {
        var json = HighchartJsonSerializer.ToChartJson(options);
        return ParsedByJson.GetOrAdd(json, static j => JObject.Parse(j));
    }
}
