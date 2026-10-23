namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule
{
    public interface IMenuViewFactory
    {
        MenuView CreateMenuViewServiceActions<TService>();
        MenuView CreateMenuViewDbContextInitializerActions();
        MenuView CreateMenuView(string name, Dictionary<string, Action> operations);
        MenuView CreateMenuView(string name, object service);
    }
}