using Console_CommonModel.CommonLib.LibViews.AppViews;
using Console_CommonModel.CommonLib.LibViews.AppViewModules;
using Console_CommonModel.CommonLib.LibViews.SharedViewControls;
using Console_CommonModel.CommonLib.LibViews.Testing;
using Console_CommonModel.CommonLib.LibViews.ViewEvents;

namespace Console_CommonModel.CommonLib.LibViews;

[Label("Тестирование представлений LibViews")]
[Description("Для каждого представления проверяется создание через parameterless конструктор без исключений.")]
public class ViewsUnit : TestingUnit
{
    public ViewsUnit(IServiceProvider provider) : base(provider)
    {

        Append(new ListViewTest(provider));
        //foreach (var (area, types) in LibViewsViewDiscovery.GetViewTypesByArea())
        //    Append(new LibViewsAreaConstructorTestsUnit(provider, area, types));

        Append(new AppViewsUnit(provider));
        Append(new AppViewModulesUnit(provider));
        Append(new SharedViewControlsUnit(provider));
        Append(new ViewEventsUnit(provider));
    }
}
