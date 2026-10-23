using Console_CommonModel.CommonLib.LibViews.AppViews.FormModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews
{

    [Label("Представление для настройки и редактирования свойств представлений")]
    public class CustomizationView : AppViewBase
    {

        [Label("Customizing View")]
        public AppViewBase CustomizingView { get => Get<AppViewBase>(nameof(CustomizingView)); set => Set(nameof(CustomizingView), value); }

        public CustomizationView() : this(new AppViewBase())
        {
        }

        public CustomizationView(AppViewBase lineChartView)
        {
            TogglePanels();

            AddPropertyChangeListener(nameof(CustomizingView), evt => SetView((AppViewBase)evt.After));
            OnLoad += (s, e) =>
            {
                SetView(CustomizingView = lineChartView);
            };
        }

        public void SetView(AppViewBase view)
        {
            LayoutFill(view);
            Right = EditView.CreateEditViewBase(view);
            Right.Width = 480;
        }

        //public override Control ToControl()
        //{
        //    return ToControl<AppViewBase>();
        //}
    }
}
