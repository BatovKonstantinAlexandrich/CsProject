using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace WinFormsApp.AppComponents.AppControls.Highcharts;

/// <summary>Сериализация моделей <see cref="Highchart"/> в JSON опций Highcharts.</summary>
public static class HighchartJsonSerializer
{
    private static readonly JsonSerializer ChartSerializer = CreateChartSerializer();
    private static readonly JsonSerializer MarkerInnerSerializer = JsonSerializer.Create(new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
    });

    private static JsonSerializer CreateChartSerializer()
    {
        return JsonSerializer.Create(new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = { new HighchartMarkerConverter(), new HighchartDelegateJsonConverter() },
        });
    }

    public static string ToChartJson(Highchart options)
    {
        if (options is null)
            return "{}";

        if (HighchartTypeRegistry.TryGetDefaultModelJson(options, out var cached))
            return cached;

        return SerializeCore(options);
    }

    internal static string SerializeCore(Highchart options)
    {
        if (options is null)
            return "{}";

        try
        {
            var token = JToken.FromObject(options, ChartSerializer);
            NormalizeToken(token);
            return token.ToString(Formatting.None);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Не удалось сериализовать модель Highcharts ({options.GetType().Name}): {ex.Message}", ex);
        }
    }

    /// <summary>Проверяет сериализацию всех корневых типов Highcharts. Пустой список — без ошибок.</summary>
    public static List<(string TypeName, string Error)> RunSerializationSelfTest()
    {
        var failures = new List<(string, string)>();
        foreach (var name in HighchartTypeRegistry.ChartTypeNames)
        {
            try
            {
                var model = HighchartTypeRegistry.CreateInstance(name);
                var json = ToChartJson(model);
                if (string.IsNullOrWhiteSpace(json) || json == "{}")
                    failures.Add((name, "пустой JSON"));
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.ToString() ?? ex.ToString();
                failures.Add((name, detail.Length > 800 ? detail[..800] : detail));
            }
        }

        return failures;
    }

    private static void NormalizeToken(JToken token)
    {
        if (token is JObject obj)
        {
            obj.Remove("highchartType");
            foreach (var child in obj.Properties().ToList())
            {
                NormalizeToken(child.Value);
                if (child.Value is JObject empty && !empty.HasValues && IsBooleanMarkerPropertyName(child.Name))
                    child.Value = new JValue(true);
            }
        }
        else if (token is JArray arr)
        {
            foreach (var item in arr)
                NormalizeToken(item);
        }
    }

    /// <summary>Пустые вложенные объекты-флаги Highcharts (polar, enabled…) → true.</summary>
    private static bool IsBooleanMarkerPropertyName(string name) =>
        name is "polar" or "inverted" or "enabled" or "shared" or "useHTML" or "outside"
        or "allowOverlap" or "endOnTick" or "showLastLabel";

    /// <summary>Пустые вложенные классы-маркеры (enabled, shared…) → <c>true</c>.</summary>
    private sealed class HighchartMarkerConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType != typeof(Highchart)
            && typeof(Highchart).IsAssignableFrom(objectType)
            && !HighchartTypeRegistry.IsRootChartType(objectType);

        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => throw new NotSupportedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            var type = value.GetType();
            if (!HasDeclaredJsonProperties(type))
            {
                writer.WriteValue(true);
                return;
            }

            var token = JToken.FromObject(value, MarkerInnerSerializer);
            if (token is JObject obj)
            {
                obj.Remove("highchartType");
                if (!obj.HasValues)
                {
                    // Пустой объект с объявленными полями (yAxis, plotOptions.column…) — {}, не true.
                    if (HasDeclaredJsonProperties(type))
                    {
                        writer.WriteStartObject();
                        writer.WriteEndObject();
                    }
                    else
                        writer.WriteValue(true);
                    return;
                }

                obj.WriteTo(writer);
            }
            else
            {
                token.WriteTo(writer);
            }
        }

        private static bool HasDeclaredJsonProperties(Type type) =>
            type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Any(p => p.GetCustomAttribute<JsonPropertyAttribute>() is not null
                    && p.GetCustomAttribute<JsonIgnoreAttribute>() is null
                    && p.GetCustomAttribute<Newtonsoft.Json.JsonIgnoreAttribute>() is null);
    }

    /// <summary>Делегаты (click, formatter…) не сериализуются в JSON.</summary>
    private sealed class HighchartDelegateJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => typeof(Delegate).IsAssignableFrom(objectType);
        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => throw new NotSupportedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            writer.WriteNull();
    }
}
