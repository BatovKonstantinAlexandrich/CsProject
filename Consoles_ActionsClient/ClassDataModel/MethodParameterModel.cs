using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp.Console.Console_ClassModule.ClassDataModel
{

    public class MethodParameterModel : HierTable<MethodModel>
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Help { get; set; }
        public List<MethodParameterModelAttribute> Attributes { get; set; } = new();
    }
}
