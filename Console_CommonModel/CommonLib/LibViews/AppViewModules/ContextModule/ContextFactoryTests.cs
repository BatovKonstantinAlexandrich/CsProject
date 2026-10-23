namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.ContextModule
{
    public class ContextFactoryTests : TestingService<ContextFactory>
    {
        public ContextFactoryTests(IServiceProvider serviceProvider)
        {

        }

        public override bool OnServiceTest(ContextFactory service)
        {
            return true;
        }
    }
}
