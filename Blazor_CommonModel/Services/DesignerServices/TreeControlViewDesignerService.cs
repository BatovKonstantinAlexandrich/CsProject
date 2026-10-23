using WinFormsApp.AppViews.ApiServices;

namespace WinFormsApp.DesignerServices
{
    [Label("Настроить иерархию")]
    internal class TreeControlViewDesignerService : DesignerBase, IControlService<TreeControlView>
    {
        public TreeControlView Control { get; set; }

        public TreeControlViewDesignerService()
        {
        }
        public TreeControlViewDesignerService(TreeControlView item)
        {
            Control = item;
        }

        public void SetControl(TreeControlView item)
        {
            this.Info(item);
        }

        protected override void Activating(ViewNode view)
        {
            AppViewBase vb = (AppViewBase)view;
            rollback.Add( vb.ContextMenu.AddContextMethods(
                "Настройка иерархии",
                vb.GetActionMap(this, new List<string>()
                {
                    nameof(CreateTreeViewBuilder)
                })
            ));
        }

        [Label("Открыть мастер настройки")]
        public void CreateTreeViewBuilder()
        {

        }
    }
}