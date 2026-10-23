namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;



[Label("Тестирование моделей и структур данных.")]
public class ModelsModuleTest : TestingUnit
{
    public ModelsModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new ModelsModuleInputPanelCreationTest(provider));
    }
}
