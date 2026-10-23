namespace BlazorDesignerApp.Web.Services.ContextMenu;

public sealed class ContextMenuEntry
{
    public required string Text { get; init; }
    public string? Icon { get; init; }
    public bool Disabled { get; init; }
    public object? Tag { get; init; }
}
