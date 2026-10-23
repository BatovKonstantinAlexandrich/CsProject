using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsApp.AppViews.ColumnsModule;

namespace WinFormsApp.AppComponents.AppControls
{
    public class ColumnsComponent : ComponentBase<ColumnsView, ColumnsControl>
    {
        public ColumnsComponent([NotNullNotEmpty] ColumnsView ViewModel, [NotNullNotEmpty] ColumnsControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            ColumnsView view = GetViewModel();
            ColumnsControl ctrl = GetControl();

        }
    }
}
