using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.Views.Events;

public sealed class ViewSelectionEventArgs
{
    public ViewKind ViewKind { get; init; }
    public string? ItemId { get; init; }
    public object? Item { get; init; }
    public bool IsAdditive { get; init; }
    public IReadOnlyCollection<string> SelectedIds { get; init; } = [];
}
