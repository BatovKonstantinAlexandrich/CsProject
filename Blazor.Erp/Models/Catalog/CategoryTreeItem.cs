namespace BlazorErp.Models.Catalog;

public class CategoryTreeItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public List<CategoryTreeItem> Children { get; set; } = [];
}
