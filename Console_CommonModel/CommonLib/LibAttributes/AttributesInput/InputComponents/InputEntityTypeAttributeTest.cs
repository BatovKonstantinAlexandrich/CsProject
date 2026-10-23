
using Console_CommonModel.CommonLib.LibAttributes;

[Label("Тестирование выбора типа сущности")]
public class InputEntityTypeAttributeTest : InputAttributeTesting<InputEntityTypeAttribute>
{
    public InputEntityTypeAttributeTest(IServiceProvider provider) : base(provider)
    {
    }
 
}