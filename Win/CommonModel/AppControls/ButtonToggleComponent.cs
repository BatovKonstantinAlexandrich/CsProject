using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsApp.AppViewModules.CheckboxModule;

namespace WinFormsApp.AppComponents.AppControls
{
    public class ButtonToggleComponent : ComponentBase<ToggleView, CheckboxControl>
    {
        public ButtonToggleComponent([NotNullNotEmpty] ToggleView ViewModel, [NotNullNotEmpty] CheckboxControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            ToggleView view = GetViewModel();
            CheckboxControl ctrl = GetControl();
            view.OnCheckedChangedListener(val =>
            {
                if (ctrl.checkbox.Checked != val)
                {
                    ctrl.checkbox.Checked = val;
                }
            });
            /*ctrl.AddPro
            (val => {
                if (ctrl.Button.Checked != val)
                {
                    ctrl.Button.Checked = val;
                }
            });*/


        }
    }
}
