namespace BlazorErp.Shared.Views.Models;

public sealed class ViewTableData
{
    public string Title { get; set; } = string.Empty;
    public IList<ViewColumnDefinition> Columns { get; set; } = [];
    public IList<IReadOnlyDictionary<string, object?>> Rows { get; set; } = [];
}
