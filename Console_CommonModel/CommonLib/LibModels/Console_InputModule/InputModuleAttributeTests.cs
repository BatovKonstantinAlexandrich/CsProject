namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.AttributeTests;

[Label("Тестирование функций ввода")]
public class InputModuleAttributeTests : TestingUnit
{
    public InputModuleAttributeTests(IServiceProvider provider) : base(provider)
    {
        Append(new GenericInputAttributeTestUnit(provider));
    }
}
