using Microsoft.AspNetCore.Components.Web;

namespace Blazor_CommonModel.Components.Context;

public sealed class ContextMenuService : IContextMenuService
{


    public ContextMenuRequest? Current { get; private set; }
    public event Action? Changed;

    public Task ShowAsync(
        IEnumerable<MenuItemItem<object>> items,
        ActionModels.MouseEventArgs mouseEvent,
        Func<MenuItemItem<object>, Task>? onItemSelected = null) =>
        ShowAsync(items, mouseEvent.X, mouseEvent.Y, onItemSelected);

    public Task ShowAsync(
        IEnumerable<MenuItemItem<object>> items,
        double clientX,
        double clientY,
        Func<MenuItemItem<object>, Task>? onItemSelected = null)
    {
        var list = items as IReadOnlyList<MenuItemItem<object>> ?? items.ToList();
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

    public void AddRadioMenu(string name, List<string> options, Action<string> onchanged)
    {
        throw new NotImplementedException();
    }

    public void AddMenuItems(string name, List<string> items, Action<string> onclick)
    {
        throw new NotImplementedException();
    }
}
