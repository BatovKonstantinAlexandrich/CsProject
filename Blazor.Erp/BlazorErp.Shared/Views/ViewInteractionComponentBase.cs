using BlazorErp.Shared.Views.Events;
using BlazorErp.Shared.Views.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorErp.Shared.Views;

/// <summary>Базовый класс представлений с поддержкой выбора и перетаскивания.</summary>
public abstract class ViewInteractionComponentBase : ComponentBase
{
    [Parameter] public ViewKind ViewKind { get; set; }
    [Parameter] public bool SelectionEnabled { get; set; } = true;
    [Parameter] public bool EnableDragDrop { get; set; }
    [Parameter] public EventCallback<ViewSelectionEventArgs> OnSelectionChanged { get; set; }
    [Parameter] public EventCallback<ViewDragDropEventArgs> OnDragDrop { get; set; }

    protected HashSet<string> SelectedIds { get; } = new(StringComparer.OrdinalIgnoreCase);

    protected async Task NotifySelectionAsync(string? itemId, object? item, bool additive = false)
    {
        if (!SelectionEnabled || !OnSelectionChanged.HasDelegate)
            return;

        if (string.IsNullOrEmpty(itemId))
        {
            SelectedIds.Clear();
        }
        else if (additive)
        {
            if (!SelectedIds.Add(itemId))
                SelectedIds.Remove(itemId);
        }
        else
        {
            SelectedIds.Clear();
            SelectedIds.Add(itemId);
        }

        await OnSelectionChanged.InvokeAsync(new ViewSelectionEventArgs
        {
            ViewKind = ViewKind,
            ItemId = itemId,
            Item = item,
            IsAdditive = additive,
            SelectedIds = SelectedIds.ToList()
        });
    }

    protected async Task NotifyDragDropAsync(
        ViewDragDropPhase phase,
        string? sourceItemId,
        string? targetItemId,
        object? sourceItem = null,
        object? targetItem = null)
    {
        if (!EnableDragDrop || !OnDragDrop.HasDelegate)
            return;

        var args = new ViewDragDropEventArgs
        {
            ViewKind = ViewKind,
            Phase = phase,
            SourceItemId = sourceItemId,
            TargetItemId = targetItemId,
            SourceItem = sourceItem,
            TargetItem = targetItem
        };

        await OnDragDrop.InvokeAsync(args);
    }

    protected string GetRowClass(string? itemId) =>
        itemId != null && SelectedIds.Contains(itemId) ? "mud-table-row-selected view-row-selected" : string.Empty;
}
