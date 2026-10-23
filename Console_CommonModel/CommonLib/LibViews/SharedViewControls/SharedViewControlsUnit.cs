using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.ImageModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.CheckboxModule;

using Console_CommonModel.CommonLib.LibViews.AppViews;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeModule;

namespace Console_CommonModel.CommonLib.LibViews.SharedViewControls
{

    /// <summary>
    /// Тестирование представлений SharedViewControls
    /// </summary>
    [Label("Тестирование представлений SharedViewControls")]
    public class SharedViewControlsUnit : TestingUnit
    {
        public SharedViewControlsUnit()
        {
            InitTests();
        }

        public SharedViewControlsUnit(IServiceProvider provider) : base(provider)
        {
            InitTests();
        }

        private void InitTests()
        {
            Append(new TextBoxTests(provider));
            Append(new CheckboxTests(provider));
            Append(new ComboboxTests(provider));
            Append(new ImageTests(provider));
            Append(new TableTests(provider));
            Append(new TreeTests(provider));
        }
    }
}
