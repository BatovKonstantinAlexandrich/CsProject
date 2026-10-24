using Console_CommonModel.CommonLib.LibData;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibDesigner;
using Console_CommonModel.CommonLib.LibModels;
using Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule;
using Console_CommonModel.CommonLib.LibViews;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib
{
    [Label("Модуль тестирования библиотек в сборки Console_CommonModel")]
    public class LibUnit : TestingUnit
    {
        public LibUnit(IServiceProvider parent) : base(parent)
        {
            Append(new LibAttrbiutesUnit(parent));
            Append(new LibDataUnit(parent));            
            Append(new LibAuthUnit(parent));
            Append(new DataModuleUnit(parent));
            Append(new DesignerUnit(parent));
            Append(new LibAuthUnit(parent));
            //Append(new ModelsUnit(parent));
            Append(new ViewsUnit(parent));
        }
    }
}
