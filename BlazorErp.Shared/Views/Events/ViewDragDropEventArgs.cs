using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.Views.Events;

public enum ViewDragDropPhase
{
    DragStart,
    DragOver,
    Drop
}

public sealed class ViewDragDropEventArgs
{
    public ViewKind ViewKind { get; init; }
    public ViewDragDropPhase Phase { get; init; }
    public string? SourceItemId { get; init; }
    public string? TargetItemId { get; init; }
    public object? SourceItem { get; init; }
    public object? TargetItem { get; init; }
    public bool Handled { get; set; }
}
