using ActionModels;

using Console_CommonModel.CommonLib.LibData.AbstractionModel;


namespace WinFormsApp.Console.Console_ClassModule.ClassDataModel
{


    public class AttributeBase<T>: HierTable<T>
        where T : BaseEntity
    {
        public string Value { get; set; }
    }


    public class MethodParameterModelAttribute : AttributeBase<MethodParameterModel>
    {
    }
    public class MethodModelAttribute : AttributeBase<MethodModel>
    {
    }
    public class ClassModelAttribute : AttributeBase<ClassModel>
    {
    }
    public class PropertyModelAttribute : AttributeBase<PropertyModel>
    {
    }
    public class AssemblyModelAttribute: AttributeBase<AssemblyModel>
    {

    }
}
