using Microsoft.AspNetCore.Components.Web;

namespace BlazorDesignerApp.Web.Services.ContextMenu;

public interface IContextMenuService
{
    ContextMenuRequest? Current { get; }
    event Action? Changed;

    Task ShowAsync(
        IEnumerable<ContextMenuEntry> items,
        MouseEventArgs mouseEvent,
        Func<ContextMenuEntry, Task>? onItemSelected = null);

    Task ShowAsync(
        IEnumerable<ContextMenuEntry> items,
        double clientX,
        double clientY,
        Func<ContextMenuEntry, Task>? onItemSelected = null);

    void Hide();
}
