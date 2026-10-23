using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibViews.ViewEvents;

namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule
{
    internal class InputModuleUnit : TestingUnit
    {
        public InputModuleUnit(IServiceProvider parent) : base(parent)
        {
            Append(new ViewEventsUnit(parent));
        }
    }
}
