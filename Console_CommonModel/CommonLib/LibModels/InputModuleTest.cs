using Console_CommonModel.CommonLib.LibModels.Console_DllModule;
using Console_CommonModel.CommonLib.LibModels.Console_InputModule.AttributeTests;

namespace Console_CommonModel.CommonLib.LibModels;

[Label("{{GetUnitLabel()}}")]
internal class InputModuleTest : TestingUnit
{
    public InputModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new InputModuleAttributeTests(provider));
    }

    public string GetUnitLabel()
    {
        return $"Тестирование моделей ввода";
    }

   
}
