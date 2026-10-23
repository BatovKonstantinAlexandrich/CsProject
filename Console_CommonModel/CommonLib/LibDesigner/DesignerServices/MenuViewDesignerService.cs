using Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibDesigner.DesignerServices
{

    [Label("Настроить меню")]
    public class MenuViewDesignerService : DesignerBase, IControlService<MenuView>
    {
        public MenuViewDesignerService()
        {
        }

        public MenuView Control { get; set; }

        public void SetControl(MenuView item)
        {
            item.AddPropertyChangeListener(nameof(item.DesignMode), evt =>
            {
                if ((bool)evt.After)
                    Activating(item);
                else Deactivate(item);
            });
        }

        protected override void Activating(ViewNode view)
        {
            rollback.Add(((AppViewBase)view).ContextMenu.AddContextMethods("Добавить", new() {
                { "Кнопку", ()=>{
                    Control.MenuItems.Add(Control.CreateMenuView("Нажать",()=>{ }));
                } },
                { "Переключатель", ()=>{
                    Control.MenuItems.Add(Control.CreateMenuView("Нажать",()=>{ }));
                } },
                { "Выпадающий список", ()=>{
                    Control.MenuItems.Add(Control.CreateMenuView("Нажать",()=>{ }));
                } },
            }));
            rollback.Add(((AppViewBase)view).ContextMenu.AddContextMethods("Расположение", new() {
                { "Слева", ()=>{ } },
                { "Справа", ()=>{ } },
            }));
        }
    }
}