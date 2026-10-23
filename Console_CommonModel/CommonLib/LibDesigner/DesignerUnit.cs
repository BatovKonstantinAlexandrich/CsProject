using Console_CommonModel.CommonLib.LibDesigner.DesignerControls;
using Console_CommonModel.CommonLib.LibDesigner.DesignerServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibDesigner
{
    public class DesignerUnit : TestingUnit
    {
        public DesignerUnit(IServiceProvider parent) : base(parent)
        {
            Append(new ViewItemContextMenuServiceTest(provider));
            Append(new DesignerControlsTest(provider));
            Append(new DesignerServicesTest(provider));
        }

    }
}
