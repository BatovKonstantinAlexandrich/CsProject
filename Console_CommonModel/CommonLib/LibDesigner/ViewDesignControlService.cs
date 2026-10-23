using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibDesigner
{
    public class ViewDesignControlService: IControlService<MenuView>
    {
        public MenuView Control { get; set; }

        public void SetControl(MenuView item)
        {
            throw new NotImplementedException();
        }
    }
}
