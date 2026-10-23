using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule
{
    public class AttributeBase : BaseEntity
    {
        public string AttributeType { get; set; }
        public string AttributeValue { get; set; }
    }
    public class ParameterAttributeModel : AttributeBase
    {
        public int ParameterModelId { get; set; }
        public ParameterModel ParameterModel { get; set; }
    }

}
