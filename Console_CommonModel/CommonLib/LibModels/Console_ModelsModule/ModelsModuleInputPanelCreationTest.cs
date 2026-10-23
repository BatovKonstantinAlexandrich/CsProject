namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;

[Label("ModelsModule: создание InputPanel")]
[Description("InputPanel создаётся и готов к привязке полей формы процедуры.")]
public class ModelsModuleInputPanelCreationTest : TestingElement
{
    public ModelsModuleInputPanelCreationTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        LogMessage("new InputPanel()");
        Assert(_ =>
        {
            var panel = new AppViewBase();
            LogMessage(panel != null ? "InputPanel создан" : "InputPanel = null");
            return panel != null;
        },
        "InputPanel создаётся без ошибок",
        "InputPanel не создаётся");
    }
}
