using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp.AppComponents.AppControls
{
    public class InputControl : ControlBase
    {
        public InputPanel input;
        public InputControl()
        {
            this.LayoutFill(input = new InputPanel());
        }


        public void Set(object target, List<string> properties)
        {
            input.Set(target, properties);
        }
    }
}

