using Microsoft.AspNetCore.Components.Web;

namespace BlazorDesignerApp.Web.Services.ContextMenu;

public sealed class ContextMenuService : IContextMenuService
{
    public ContextMenuRequest? Current { get; private set; }
    public event Action? Changed;

    public Task ShowAsync(
        IEnumerable<ContextMenuEntry> items,
        MouseEventArgs mouseEvent,
        Func<ContextMenuEntry, Task>? onItemSelected = null) =>
        ShowAsync(items, mouseEvent.ClientX, mouseEvent.ClientY, onItemSelected);

    public Task ShowAsync(
        IEnumerable<ContextMenuEntry> items,
        double clientX,
        double clientY,
        Func<ContextMenuEntry, Task>? onItemSelected = null)
    {
        var list = items as IReadOnlyList<ContextMenuEntry> ?? items.ToList();
        if (list.Count == 0)
        {
            Hide();
            return Task.CompletedTask;
        }

        Current = new ContextMenuRequest
        {
            Items = list,
            ClientX = clientX,
            ClientY = clientY,
            OnItemSelected = onItemSelected
        };

        Changed?.Invoke();
        return Task.CompletedTask;
    }

    public void Hide()
    {
        if (Current is null)
            return;

        Current = null;
        Changed?.Invoke();
    }
}
