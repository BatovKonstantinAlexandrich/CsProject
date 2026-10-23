namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule
{

    /// <summary>
    /// Составляет меню по данным из файла
    /// </summary>
    public class MainMenuService
    {

        public static List<MenuView> ReadMenuModelFromJsonFile(string filename = "mainmenu.json")
        {
            if (!File.Exists(filename))
                throw new Exception($"Не найден файл: {filename}");
            var json = File.ReadAllText(filename);
            Func<MenuModel, MenuView> convert = null;
            convert = model =>
            {
                List<MenuView> children = model.Links.Select(convert).ToList();
                return new MenuView()
                {
                    Text = model.Label,
                    Command = model.Command,
                    Icon = model.Icon,
                    Shortcut = model.Shortcut,
                    MenuItems = new(children)
                };
            };
            var menus = System.Text.Json.JsonSerializer.Deserialize<List<MenuModel>>(json).Select(convert).ToList();
            return menus;
        }



        internal static IEnumerable<MenuView> ReadMenuModelFromJsonFile(string filename, Action<string> onCommand)
        {
            if (!File.Exists(filename))
                throw new Exception($"Не найден файл: {filename}");
            var json = File.ReadAllText(filename);
            Func<MenuModel, MenuView> convert = null;
            convert = model =>
            {
                List<MenuView> children = model.Links.Select(convert).ToList();
                var view = new MenuView()
                {
                    Text = model.Label,
                    Command = model.Command,
                    Icon = model.Icon,
                    Shortcut = model.Shortcut,
                    MenuItems = new(children)
                };
                view.OnCommandExecute = cmd =>
                {
                    onCommand(cmd);
                };
                return view;
            };
            var menus = System.Text.Json.JsonSerializer.Deserialize<List<MenuModel>>(json).Select(convert).ToList();
            menus.ForEach(menu => menu.MenuItems.ToList().ForEach(link => link.OnCommandExecute = cmd => { onCommand(cmd); }));
            return menus;
        }
    }
}
