using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsApp.AppViews.NavModule;

using WinFormsApp.AppComponents.AppControls;

namespace WinFormsApp.AppComponents.AppControls
{
    public class NavListComponent : ComponentBase<NavListView, NavListControl>
    {

        public NavListComponent() : base(new(), new()) { }

        public NavListComponent([NotNullNotEmpty] NavListView ViewModel, [NotNullNotEmpty] NavListControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
        }
    }
}
