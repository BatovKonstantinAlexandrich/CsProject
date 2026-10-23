using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsApp.AppViews.BaseComponent;
using WinFormsApp.AppViews.ToolsModule;

namespace WinFormsApp.AppComponents.AppControls
{
    public class ToolsComponent : ComponentBase<ToolsView, ToolsControl>
    {

        public ToolsComponent() : base(new(), new()) { }

        public ToolsComponent([NotNullNotEmpty] ToolsView ViewModel, [NotNullNotEmpty] ToolsControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            var toolsView = GetViewModel();
            var toolsControl = GetControl();
            toolsView.Controls.OnAdded(OnViewAdded);
            toolsView.Controls.OnRemoved(OnViewRemoved);
            int i = 0;
            foreach (AppViewBase ctrl in toolsView.Controls)
            {
                OnViewAdded(i, ctrl);
                i++;
            }
        }

        private void OnViewAdded(int index, object p)
        {
            AppViewBase view = (AppViewBase)p;
            GetControl().Controls.Add(view.ToControl());
        }

        private void OnViewRemoved(int index, object p)
        {
            AppViewBase view = (AppViewBase)p;
            GetControl().Controls.Remove(view.ToControl());
        }
    }
}
