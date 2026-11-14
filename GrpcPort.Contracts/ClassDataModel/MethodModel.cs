using System.Collections.Generic;

namespace WinFormsApp.Console.Console_ClassModule.ClassDataModel
{
    public class MethodModel : HierTable<ClassModel>
    {
        public MethodModel()
        {
            
        }

        public string Type { get; set; }

        public string? Label { get;set; }
        public string? Icon { get;set; }
        public string? Help { get;set; }
        public string? Description { get;set; }
        public string? Body { get;set; }

        public List<MethodParameterModel> Parameters { get; set; } = new();
        public List<MethodModelAttribute> Attributes { get; set; } = new();
        public string? OwnerClientId { get; set; }

        public static MethodModel FromJson(string methodJson)
        {
            throw new NotImplementedException();
        }
    }
}
