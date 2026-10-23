using Console_CommonModel.CommonLib.LibViews.AppViewModules.ImageModule;

using Console_CommonModel.CommonLib.LibViews.AppViews;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules
{
    /// <summary>
    /// Тестирование представлений AppViewModules
    /// </summary>
    [Label("Тестирование представлений AppViewModules")]
    public class AppViewModulesUnit : TestingUnit
    {
        public AppViewModulesUnit()
        {
            InitTests();
        }

        public AppViewModulesUnit(IServiceProvider provider) : base(provider)
        {
            InitTests();
        }

        private void InitTests()
        {

            Append(new ImageTests(provider));
            Append(new TableTests(provider));
            Append(new TreeTests(provider));
        }

    }
}
