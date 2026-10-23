using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels
{

    [Label("Панель компонента")]
    public class PaneItem : ViewComponentStyle
    {
        public string ToString()
        {
            return GetHashCode() + "";
        }
    }
}
