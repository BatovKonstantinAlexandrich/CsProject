namespace BlazorErp.Shared.Views.Models;

public sealed class ViewColumnDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Format { get; set; }
    public bool ShowSum { get; set; }
    public int SortOrder { get; set; }
}
