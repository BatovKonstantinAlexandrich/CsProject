namespace Console_CommonModel.CommonLib.LibViews.AppViews
{
    [Label($"Тестирование {nameof(AppViewRoot)}")]
    public class PagesTests : TestingElement
    {
        public PagesTests() : this(AppProviderService.GetInstance()) { }
        public PagesTests(IServiceProvider serviceProvider)
        {

        }

        public override void OnTest()
        {
        }
    }
}
