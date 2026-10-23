using WinFormsApp.AppViews.ApiServices;
using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.DesignerControls
{

    /// <summary>
    /// Реализует логику подключение средств разработчика    
    /// </summary>
    [Label("Управление службами разработки")]
    public class ViewServicesControlDesigner : DesignerBase
    {
        protected override void Activating(ViewNode view)
        {

            //включение режима разработки
            var contextMenus = new ContextMenuFactory().CreateContextAction(
               "Режим отладки", () =>
               {
                   DesignMode = !DesignMode ? true : false;
               });
            ((AppViewBase)view).ContextMenu.Items.Add(contextMenus);
            ((AppViewBase)view).TogglePanels();
            rollback.Add(() =>
            {
                ((AppViewBase)view).TogglePanels();
            });
            rollback.Add(() =>
            {
                ((AppViewBase)view).ContextMenu.Items.Remove(contextMenus);
            });

            /*
            //подключение служб
            var services = ((AppViewBase)view).GetViewServices();

            //контекстное меню
            ((AppViewBase)view).contextMenuServices.AddRange(services);
            this.rollback.Add(() => {
                foreach(var service in services)
                {
                    ((AppViewBase)view).contextMenuServices.Remove(service);
                }                                
            }); */
        }


    }
}
