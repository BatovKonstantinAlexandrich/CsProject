using System;
using System.Windows.Forms;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViewModules.MenuModule;
using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    public class MenuComponent : ComponentBase<MenuView, MenuControl>
    {
        private bool _subscribedRootPropertyChange;
        private bool _subscribedMenuItemsList;

        public MenuComponent() : base(new(), new()) { }
        public MenuComponent([NotNullNotEmpty] MenuView Model, [NotNullNotEmpty] MenuControl View) : base(Model, View)
        {
        }

        public override void Bind()
        {
            var menuView = GetViewModel();
            var menuControl = GetControl();
            menuView.control = menuControl;
            menuControl.viewModel = menuView;
            menuView.AddPropertyChangeListener(nameof(menuView.Text), evt => { 
                menuControl.menuStrip.Text = $"{evt}";
            });

            if (!_subscribedRootPropertyChange)
            {
                menuView.OnChange += _ => RebuildMenuStrip();
                _subscribedRootPropertyChange = true;
            }

            RebuildMenuStrip();
        }

        private void RebuildMenuStrip()
        {
            var service = new MenuControlService();
            var menuView = GetViewModel();
            var menuControl = GetControl();

            MenuControlService.ConfigureMenuStripForIcons(menuControl.menuStrip);
            menuControl.Clear();

            foreach (MenuView menu in menuView.MenuItems)
            {
                var item = service.ToWinFormsMenuItem(menu, (cmd) =>
                {
                    menu.OnCommandExecute?.Invoke(cmd);
                    menuView.OnCommandExecute?.Invoke(cmd);
                });
                item.Alignment = ToStripAlignment(menu.Alignment);
                menu.AddPropertyChangeListener(nameof(MenuView.Alignment), evt =>
                    item.Alignment = ToStripAlignment(evt.After?.ToString()));
                MenuControlService.ApplyIconPresentationRecursive(item);
                menuControl.menuStrip.Items.Add(item);
            }

            if (!_subscribedMenuItemsList)
            {
                menuView.MenuItems.OnChanged += OnRootMenuItemsChanged;
                _subscribedMenuItemsList = true;
            }

            menuView.RegisterShortcutsRecursive();
        }

        private void OnRootMenuItemsChanged(object sender, IChangingListEvent evt)
        {
            var menuView = GetViewModel();
            var menuControl = GetControl();
            var service = new MenuControlService();
            var menu = (MenuView)evt.Target.Item;

            switch (evt.Target.Action)
            {
                case "added":
                    var stripItem = service.ToWinFormsMenuItem(menu, (cmd) =>
                    {
                        menu.OnCommandExecute?.Invoke(cmd);
                        menuView.OnCommandExecute?.Invoke(cmd);
                    });
                    stripItem.Alignment = ToStripAlignment(menu.Alignment);
                    menu.AddPropertyChangeListener(nameof(MenuView.Alignment), e =>
                        stripItem.Alignment = ToStripAlignment(e.After?.ToString()));
                    MenuControlService.ApplyIconPresentationRecursive(stripItem);
                    menuControl.menuStrip.Items.Add(stripItem);
                    break;
                case "removed":
                    if (evt.Target.Index >= 0 && evt.Target.Index < menuControl.menuStrip.Items.Count)
                        menuControl.menuStrip.Items.RemoveAt(evt.Target.Index);
                    break;
            }

            menuView.RegisterShortcutsRecursive();
        }

        private static ToolStripItemAlignment ToStripAlignment(string alignment) =>
            string.Equals(alignment, "right", StringComparison.OrdinalIgnoreCase)
                ? ToolStripItemAlignment.Right
                : ToolStripItemAlignment.Left;
    }
}
