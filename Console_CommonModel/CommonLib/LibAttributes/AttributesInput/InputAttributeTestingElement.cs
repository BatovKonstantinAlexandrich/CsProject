using Console_CommonModel.CommonLib.LibAttributes;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

using Console_CommonModel.CommonLib.LibViews.AppViews.InputModule;

/// <summary>
/// Базовый класс тестирования атрибута ввода
/// </summary>
/// <typeparam name="T">тип атрибута</typeparam>
public class InputAttributeTestingElement<TInputAttribute> : TestingElement where TInputAttribute : BaseInputAttribute
{

    public InputAttributeTestingElement(IServiceProvider provider) : base(provider)
    {
    }
   

    public override void OnTest()
    {
        EditView.Fields.Clear();

        foreach (var kv in typeof(TestModel).GetPropertyAttributes(nameof(TestModel.TestProperty)).Keys)
        {
            typeof(TestModel).RemovePropertyAttribute(
                nameof(TestModel.TestProperty),
                kv);
        }


        typeof(TestModel).AddPropertyAttribute(nameof(TestModel.TestProperty),
           typeof(TInputAttribute).GetTypeName(), "");
        this.Info(typeof(TestModel).GetPropertyAttributes(nameof(TestModel.TestProperty)).Keys);
        EditView.CreateEditView(new TestModel(), new List<string>() { nameof(TestModel.TestProperty) })
        .ShowDialog(
            $"[{typeof(TInputAttribute).GetTypeName()}]. {typeof(TInputAttribute).GetLabel()}"
        );
        typeof(TestModel).RemovePropertyAttribute(
            nameof(TestModel.TestProperty),
            typeof(TInputAttribute).GetTypeName());


        this.Info(typeof(TestModel).GetPropertyAttributes(nameof(TestModel.TestProperty)));
    }
}