
using Blazor_MyApp.Application;

namespace Blazor_MyApp
{
    public class MyAppUnit : TestingUnit
    {
        public MyAppUnit(IServiceProvider parent) : base(parent)
        {
            this.Append(new ApplicationUnit(parent));
        }
    }

    public class ApplicationUnit : TestingUnit
    {
        public ApplicationUnit(IServiceProvider provider) : base(provider)
        {
            this.Append(new AuthorizationUnit(provider));
            this.Append(new UserAgentUnit(provider));
            this.Append(new WebComponentsFactoryTest(provider));

        }
    }
}
