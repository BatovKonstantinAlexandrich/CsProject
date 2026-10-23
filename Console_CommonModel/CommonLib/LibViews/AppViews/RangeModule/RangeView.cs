using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.RangeModule
{

    /// <summary>
    /// Контролы для установки диапазона значений
    /// </summary>    
    public class RangeView<TView> : AppViewBase
        where TView : AppViewBase
    {
        private readonly AppViewBase minView;
        private readonly AppViewBase maxView;

        [NotNullNotEmpty]
        private string minValue { get; set; } = null;

        [NotNullNotEmpty]
        private string maxValue { get; set; } = null;

        public RangeView()
        {
            minView = typeof(TView).New<AppViewBase>();
            maxView = typeof(TView).New<AppViewBase>();
            Add(minView);
            Add(maxView);
        }
    }
}
