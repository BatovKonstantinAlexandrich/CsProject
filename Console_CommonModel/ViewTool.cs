using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WinFormsApp.API
{
    public abstract class ViewTool: DesignerBase, IControlService<DesignerView>
    {
        protected ViewTool() : base() { }

        public ViewTool(DesignerView control): base(control) 
        {
            this.Control = control;
        }

        public DesignerView Control { get; set; }

        public void SetControl(DesignerView item)
        {
            Control = item;
        }
    }
}
