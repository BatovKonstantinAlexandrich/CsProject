using Console_CommonModel.CommonLib.LibViews.AppViews;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule
{
    [Label($"Тестирование {nameof(TextBoxView)}")]
    public class TextBoxTests : ViewTesting<TextBoxView>
    {
        public TextBoxTests()
        {
        }

        public TextBoxTests(IServiceProvider provider) : base(provider)
        {
        }
    }
}
