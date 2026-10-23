namespace BlazorErp.Shared.Views.Models;

public sealed class ViewItemNode
{
    public string Id { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Icon { get; set; }
    public IReadOnlyDictionary<string, object?> Data { get; set; } = new Dictionary<string, object?>();
    public IList<ViewItemNode> Children { get; set; } = [];
}
