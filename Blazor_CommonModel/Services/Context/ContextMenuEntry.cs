using MudBlazor;

namespace Blazor_CommonModel.Components.Context;

 
public static class ContextViewExtension
{

    
    public static MenuItemItem<object> ToMenuItem(this ContextView contextView)
    {
        var children = contextView.Items.Select(x => x.ToMenuItem()).ToList();
        return new MenuItemItem<object>()
        {
            Icon = Icons.Material.Filled.Info,
            Text = contextView.Text,
            Children = children
        };
    }
    public static MenuItemItem<object> ToContextMenuEntry(this ContextView contextView)
    {
        return new MenuItemItem<object>()
        {
            Icon = Icons.Material.Filled.Info,
            Text = contextView.Text,
            OnClick = contextView.OnClick,
            Todo = contextView.OnClick
        };
    }
}