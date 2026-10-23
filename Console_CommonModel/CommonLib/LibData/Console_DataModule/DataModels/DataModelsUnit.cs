using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageService;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels
{
    public class DataModelsUnit : TestingUnit
    {
        public DataModelsUnit()
        {
            Append(new MessageServiceTest());
            Append(new MessageWebApiTest());
        }
    }
}
