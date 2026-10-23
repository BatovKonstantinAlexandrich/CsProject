namespace Blazor_CommonModel.Components.Context;

public sealed class ContextMenuRequest
{
    public ContextView View { get; set; }
    public required IReadOnlyList<MenuItemItem<object>> Items { get; init; }
    public double ClientX { get; init; }
    public double ClientY { get; init; }
    public Func<MenuItemItem<object>, Task>? OnItemSelected { get; init; }
}
