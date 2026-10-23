public interface IMenuBuilder
{
    public MenuItemModel CreateMenu(string Text, params MenuItemModel[] Items);
    public MenuItemModel CreateMenuSelect(MenuItemModel Parent, string Text, params MenuItemModel[] Items);
}
