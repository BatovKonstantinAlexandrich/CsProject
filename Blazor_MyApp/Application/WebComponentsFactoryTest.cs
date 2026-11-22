namespace Blazor_MyApp.Application
{
    internal class WebComponentsFactoryTest : TestingElement
    {
        public WebComponentsFactoryTest(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            this.Assert(p => true, "","");
        }
    }
}