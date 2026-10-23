[Label("Выполнение динамических операция")]
public class ActionExecuterTest : TestingElement
{
    public ActionExecuterTest() : this(AppProviderService.GetInstance())
    {
    }

    public ActionExecuterTest(IServiceProvider provider) : base(provider)
    {
    }

    public override void OnTest()
    {
        this.Assert(p => {
            var viewBase = new AppViewBase();
            viewBase.LayoutFill(new CheckboxView());
            viewBase.LayoutFill(new ButtonView());

            viewBase.TryActionExecute(nameof(viewBase.GoBack), new object[0]);
            return viewBase.Controls.FirstOrDefault() is CheckboxView;
        },
        "Реализована поддержка динамическим выполнением операций объекта",
        "Не реализована поддержка динамическим выполнением операций объекта");
    }
}