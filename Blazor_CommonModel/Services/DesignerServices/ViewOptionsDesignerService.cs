
using WinFormsApp.AppViews.ApiServices;
using WinFormsApp.AppViews.EditModule;
using WinFormsApp.Views;

namespace WinFormsApp.DesignerServices
{
    /// <summary>
    /// Управление свойствами представления
    /// </summary>
    [Label("Настроить представление")]
    public class ViewOptionsDesignerService : DesignerBase, IControlService<ViewOptions>
    {
        public ViewOptions Control { get; set; }
        public ViewOptionsDesignerService() { }

        public ViewOptionsDesignerService(ViewOptions view)
        {
            this.Control = view;
        }

        public override MenuView CreateDesignerMenuView()
        {
            var menuView = base.CreateDesignerMenuView();
            menuView.AddMenuActions("Мастер", new() {
                { "Настройка DragAndDrop",    ()=>{
                    ShowDragAndDropMasterDialog();
                } },
                { "Настройка действий",       ()=>{
                    ShowUserActionsMasterDialog();
                } }
            });
            return menuView;
        }

        private void ShowUserActionsMasterDialog()
        {
            var view = new EventsBindingsEditorView();
            view.Bind((AppViewBase)this.Control);
            view.ToSubmit((success, p) => {
                view.ToControl().GetAppForm().Close();
            }).ShowDialog("Настройка действий пользователя");
        }

        private void ShowDragAndDropMasterDialog()
        {
            var view = new AppViewBase();
            view.ToSubmit((success, p) => {
                view.ToControl().GetAppForm().Close();
            }).ShowDialog("Настройка DragAndDrop");
        }

        public void SetControl(ViewOptions item)
        {
            this.Info(Control);
        }

        protected override void Activating(ViewNode view)
        {
            if (view is AppViewBase vb)
            {
                var ctx = this.CreateDesignerContextView();
                if (ctx.Items.Count() == 0)
                    return;
                vb.ContextMenu.Items.Add(ctx);
                this.rollback.Add(() => { vb.ContextMenu.Items.Remove(ctx); });
            }
        }

 
    }
}