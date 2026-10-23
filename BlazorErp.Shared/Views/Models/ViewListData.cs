namespace BlazorErp.Shared.Views.Models;

public sealed class ViewListData
{
    public string Title { get; set; } = string.Empty;
    public IList<ViewItemNode> Items { get; set; } = [];
}
