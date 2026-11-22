namespace Blazor_MyApp.Application
{
    public class AuthorizationUnit : TestingElement
    {
        public AuthorizationUnit(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            this.Assert(p => true, "", "");

        }
    }
}