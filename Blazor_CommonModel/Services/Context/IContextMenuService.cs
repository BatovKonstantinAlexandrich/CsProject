using Microsoft.AspNetCore.Components.Web;

namespace Blazor_CommonModel.Components.Context;

public interface IContextMenuService
{
    ContextMenuRequest? Current { get; }
    event Action? Changed;

    Task ShowAsync(
        IEnumerable<MenuItemItem<object>> items,
        ActionModels.MouseEventArgs mouseEvent,
        Func<MenuItemItem<object>, Task>? onItemSelected = null);

    Task ShowAsync(
        IEnumerable<MenuItemItem<object>> items,
        double clientX,
        double clientY,
        Func<MenuItemItem<object>, Task>? onItemSelected = null);

    void Hide();

    public void AddRadioMenu(string name, List<string> options, Action<string> onchanged);
    public void AddMenuItems(string name, List<string> items, Action<string> onclick);
}
