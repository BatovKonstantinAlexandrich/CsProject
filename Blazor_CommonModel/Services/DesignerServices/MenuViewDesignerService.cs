

using WinFormsApp.AppViewModules.MenuModule;
using WinFormsApp.AppViews.ApiServices;

namespace WinFormsApp.DesignerServices
{

    [Label("Настроить меню")]
    public class MenuViewDesignerService: DesignerBase, IControlService<MenuView>
    {
        public MenuViewDesignerService()
        {
        }

        public MenuView Control { get; set; }

        public void SetControl(MenuView item)
        {
            item.AddPropertyChangeListener(nameof(item.DesignMode), evt => {
                if ((bool)evt.After)
                    this.Activating(item);
                else this.Deactivate(item);
            });
        }

        protected override void Activating(ViewNode view)
        {
            rollback.Add(((AppViewBase)view).ContextMenu.AddContextMethods("Добавить", new() {
                { "Кнопку", ()=>{                    
                    this.Control.MenuItems.Add(this.Control.CreateMenuView("Нажать",()=>{ }));
                } },
                { "Переключатель", ()=>{
                    this.Control.MenuItems.Add(this.Control.CreateMenuView("Нажать",()=>{ }));
                } },
                { "Выпадающий список", ()=>{
                    this.Control.MenuItems.Add(this.Control.CreateMenuView("Нажать",()=>{ }));
                } },
            }));
            rollback.Add(((AppViewBase)view).ContextMenu.AddContextMethods("Расположение", new() {
                { "Слева", ()=>{ } },
                { "Справа", ()=>{ } },
            }));
        }
    }
}