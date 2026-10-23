using Console_CommonModel.CommonLib.LibAttributes;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

/// <summary>
/// [InputConstructor<Control>()]
/// </summary>
[Label("Тестирование ввода через конструктор")]
public class InputConstructorAttributeTest : InputAttributeTesting<InputConstructorAttribute<AppViewBase>>
{
    public InputConstructorAttributeTest(IServiceProvider provider) : base(provider)
    {
    }
    
}