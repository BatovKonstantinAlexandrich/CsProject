using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;


namespace Console_CommonModel.CommonLib.LibViews.AppViews.TypedModule
{
    [Label($"Тестирование {nameof(TypedView)}")]
    public class TypedTests : TestingElement
    {

        public TypedTests() : this(AppProviderService.GetInstance())
        {

        }

        public TypedTests(IServiceProvider serviceProvider)
        {

        }

        public override void OnTest()
        {
            Assert(p =>
            {


                var pane = new AppViewBase();
                var view = new TypedView();
                var combobox = new ComboboxView();
                combobox.Options.AddRange(new InputConstructorAttribute<AppViewBase>().GetExtendedTypesNames());
                combobox.OnSelected += (text) =>
                {
                    view.ViewModel = text.ToType().New<AppViewBase>();
                };
                pane.LayoutFill(view);
                pane.Top = combobox;

                pane.RunAtForm();
                return true;
            },
            "",
            "");
        }
    }
}
