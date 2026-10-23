using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;
namespace Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel
{

    public class ClassModelBuilder
    {
        public string Title { get; set; }

        public ClassModel classModel { get; set; }

        public void AddProperty(string name, string type, string value, List<PropertyModelAttribute> attributes)
        {
            classModel.Properties.Add(new PropertyModel()
            {
                Name = name,
                Type = type,
                Attributes = attributes
            });
        }


        public void RemoveProperty(string name)
        {
            classModel.Properties.Remove(classModel.Properties.FirstOrDefault(p => p.Name == name));
        }

        public void AddMethod(string name, List<MethodModelAttribute> attributes)
        {
            classModel.Methods.Add(new MethodModel()
            {
                Name = name,
                Attributes = attributes
            });
        }

        public void RemoveMethod(string name)
        {
            classModel.Methods.Remove(classModel.Methods.FirstOrDefault(m => m.Name == name));
        }


    }
}