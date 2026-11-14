using System;
using System.Collections.Generic;
namespace WinFormsApp.Console.Console_ClassModule.ClassDataModel
{
    public class PropertyModel : HierTable<ClassModel>
    {
        public string Type { get; set; }        
        public string? Value { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        public List<PropertyModelAttribute> Attributes { get; set; } = new();
    }
}
