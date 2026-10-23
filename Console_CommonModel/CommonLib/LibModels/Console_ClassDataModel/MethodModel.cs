using System.Collections.Generic;

namespace Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel
{
    public class MethodModel : HierTable<ClassModel>
    {
        public MethodModel()
        {

        }

        public string Type { get; set; }

        public override string? Label { get; set; }
        public override string? Icon { get; set; }
        public override string? Help { get; set; }
        public override string? Description { get; set; }
        public string? Body { get; set; }

        public List<MethodParameterModel> Parameters { get; set; } = new();
        public List<MethodModelAttribute> Attributes { get; set; } = new();


    }
}
