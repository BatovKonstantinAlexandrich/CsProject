namespace BlazorErp.Shared.Views.Models;

/// <summary>Динамическая ссылка на представление (ключ + параметры запроса).</summary>
public sealed class ViewDynamicLink
{
    public string Key { get; set; } = string.Empty;
    public IReadOnlyDictionary<string, object?> Query { get; set; } = new Dictionary<string, object?>();

    public static ViewDynamicLink Parse(string key, IReadOnlyDictionary<string, object?>? query = null) =>
        new() { Key = key, Query = query ?? new Dictionary<string, object?>() };
}
