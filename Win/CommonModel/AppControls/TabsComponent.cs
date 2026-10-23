using MaterialSurface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsApp.AppViews.TabsModule;

namespace WinFormsApp.AppComponents.AppControls
{
    public class TabsComponent : ComponentBase<TabsView, TabsControl>
    {
        public TabsComponent([NotNullNotEmpty] TabsView ViewModel, [NotNullNotEmpty] TabsControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();
            var factory = new ComponentsFactory();

            view.Tabs.OnChanged += (sender, evt) =>
            {
                switch (evt.Target.Action)
                {
                    case "added":
                        var tabCtrl = factory.CreateControl(evt.Target.Value);
                        ctrl.AddTab(evt.Target.Key, tabCtrl);
                        break;
                    case "removed":
                        ctrl.RemoveTab(evt.Target.Key);
                        break;
                }
            };
            foreach (var tab in view.Tabs)
            {
                var pctrl = factory.CreateControl(tab.Value);
                ctrl.AddTab(tab.Key, pctrl);
            }
        }
    }
}
