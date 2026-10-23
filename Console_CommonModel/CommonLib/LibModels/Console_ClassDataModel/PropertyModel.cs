using System;
using System.Collections.Generic;
namespace Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel
{
    public class PropertyModel : HierTable<ClassModel>
    {
        public string Type { get; set; }
        public string? Value { get; set; }
        public override string? Label { get; set; }
        public override string? Description { get; set; }

        public List<PropertyModelAttribute> Attributes { get; set; } = new();
    }
}
