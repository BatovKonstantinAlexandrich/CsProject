using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel
{

    public class MethodParameterModel : HierTable<MethodModel>
    {
        public string Type { get; set; }
        public string? Value { get; set; }
        public override string? Label { get; set; }
        public override string? Description { get; set; }
        public override string? Help { get; set; }
        public List<MethodParameterModelAttribute> Attributes { get; set; } = new();
    }
}
