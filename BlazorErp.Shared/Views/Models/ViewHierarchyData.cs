namespace BlazorErp.Shared.Views.Models;

public sealed class ViewHierarchyData
{
    public string Title { get; set; } = string.Empty;
    public IList<ViewItemNode> Roots { get; set; } = [];
}
