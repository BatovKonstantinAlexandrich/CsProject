using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;
using Console_CommonModel.CommonLib.LibViews.AppViews;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.ComboboxModule
{
    [Label($"Тестирование {nameof(ComboboxView)}")]
    public class ComboboxTests : ViewTesting<ComboboxView>
    {
        public ComboboxTests() : base()
        {
        }

        public ComboboxTests(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            base.OnTest();
            var c = new ComboboxView();
            c.Options.Add("1");
            c.OnChange += (evt) =>
            {
                c.Options.Add(c.Options.Count().ToString());
            };
            c.ToControl().ShowDialog();
        }
    }
}
