using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    public class PanelsComponent : ComponentBase<AppViewBase, PanelsControl>
    {
        public PanelsComponent([NotNullNotEmpty] AppViewBase ViewModel, [NotNullNotEmpty] PanelsControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();

        }
    }
}
