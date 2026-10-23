using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using WinFormsApp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.API
{
    public abstract class ViewTool: DesignerBase 
    {
        protected ViewTool() : base() { }

        public ViewTool(AppViewBase control): base(control) 
        {
             
        }
         
        public void SetControl(AppViewBase item)
        {
             
        }
    }
}
