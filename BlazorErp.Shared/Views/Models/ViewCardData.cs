namespace BlazorErp.Shared.Views.Models;

public sealed class ViewCardData
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? ImageUrl { get; set; }
    public IReadOnlyDictionary<string, object?> Fields { get; set; } = new Dictionary<string, object?>();
}
