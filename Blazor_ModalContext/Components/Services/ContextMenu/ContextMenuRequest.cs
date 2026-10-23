namespace BlazorDesignerApp.Web.Services.ContextMenu;

public sealed class ContextMenuRequest
{
    public required IReadOnlyList<ContextMenuEntry> Items { get; init; }
    public double ClientX { get; init; }
    public double ClientY { get; init; }
    public Func<ContextMenuEntry, Task>? OnItemSelected { get; init; }
}
