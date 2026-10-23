

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.LineModule
{
    [Label("Панель размещения в горизонтальную линию")]
    public class LineView : AppViewBase
    {
        public ChangingList<LineViewItem> lineViewLayouts = new();
        [Label("In Line")]
        [InputBool]
        public bool InLine { get; set; } = true;

        public class LineViewItem : ChangeSupport
        {

            [Label("view")]
            public AppViewBase view { get; set; }
            [InputBool]
            [Label("fill")]
            public bool fill { get; set; }
        }

        public LineView(params (AppViewBase, bool)[] elements)
        {
            foreach (var element in elements)
            {
                lineViewLayouts.Add(new()
                {
                    view = element.Item1,
                    fill = element.Item2
                });
            }
            MinHeight = 32;
            Height = 50;
            Width = 400;
        }

        public LineView()
        {
            Margin = Padding = 0;
        }

        public void Add(AppViewBase view, bool fill)
        {
            lineViewLayouts.Add(new()
            {
                view = view,
                fill = fill
            });
        }
    }
}
