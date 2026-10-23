
using Console_CommonModel.CommonLib.LibAttributes;

[Label("Тестирование атрибутов ввода через элементы управления")]
public class InputControlTypeAttributeTest : InputAttributeTesting<InputControlTypeAttribute>
{
    public InputControlTypeAttributeTest(IServiceProvider sp) : base(sp)
    {
    }

}
     