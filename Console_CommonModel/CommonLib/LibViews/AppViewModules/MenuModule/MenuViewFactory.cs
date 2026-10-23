using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule
{
    public class MenuViewFactory : IMenuViewFactory
    {

        public MenuView CreateMenuView(string name, Dictionary<string, Action> operations)
        {
            var menu = new MenuView() { Text = name };
            foreach (var operation in operations)
            {
                var todo = operation.Value;
                var item = new MenuView() { Text = operation.Key };
                item.OnClick += (s, e) =>
                {
                    todo();
                };
                menu.MenuItems.Add(item);
            }
            return menu;
        }

        public MenuView CreateMenuView(string name, object service)
        {
            var menu = new MenuView() { Text = name };
            foreach (var methodName in service.GetType().GetOwnMethodNames())
            {
                var label = service.GetType().GetMethodLabel(methodName);
                Action todo = () =>
                {
                    try
                    {
                        service.Invoke(name, new object[0]);
                    }
                    catch (Exception ex)
                    {
                        this.Error(ex);
                    }
                };
                var item = new MenuView() { Text = label };
                item.OnCommandExecute += cmd =>
                {
                    todo();
                };
                menu.MenuItems.Add(item);
            }
            return menu;
        }

        public MenuView CreateMenuViewServiceActions<TService>()
        {
            var menu = new MenuView();
            return menu.CreateMenuActions(
                typeof(TService).GetLabel(),
                new(menu.GetActionMap(
                    new List<object>() { menu.GetService<TService>() }, typeof(TService)))
            );
        }

        public MenuView CreateMenuViewDbContextInitializerActions()
        {
            var menu = new MenuView();
            return menu.CreateMenuViewDbContextInitializerActions();
        }
    }
}
