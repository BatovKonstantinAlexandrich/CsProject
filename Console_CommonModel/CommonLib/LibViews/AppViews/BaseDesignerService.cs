using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews
{
    [Label("Базовая служба проектировщика")]
    public class BaseDesignerService : DesignerBase
    {
        public BaseDesignerService() : base() { }

        public BaseDesignerService(AppViewBase view)
        {
            this.view = view;
        }

        public AppViewBase view { get; }

        protected override void Activating(ViewNode view)
        {
            //var viewBase = (AppViewBase)view;
            //viewBase.contextMenuServices.Add(this);
            //var actionMap = contextFactory.GetActionMap(view, typeof(BaseDesignerService));
            //AddContextServiceMethods("Проектирование", view);
        }

    }
}
