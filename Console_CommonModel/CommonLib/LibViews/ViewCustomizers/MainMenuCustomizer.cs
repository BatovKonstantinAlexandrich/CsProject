using Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.ViewCustomizers
{

    [Label("Мастер настройки главного меню")]
    [Description("Добавление пунктов меню, установка параметров расположения, отображения и управления" +
        " элементами меню")]
    public class MainMenuCustomizer : AppViewBase
    {
        private MenuView menuView = null;

        public MainMenuCustomizer(MenuView menuView)
        {
            this.menuView = menuView;
        }

        public MainMenuCustomizer() : this(new MenuView())
        {
        }
    }

}