using System.Windows.Forms;
using WinFormsApp.AppViewModules.HighchartsModule;
using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.DesignerServices;

/// <summary>
/// Гарантирует видимые размеры представлений на канве конструктора после создания WinForms-контрола.
/// </summary>
public static class DesignerViewSizeHelper
{
    public const int MinVisibleSize = 48;
    public const int DefaultMinWidth = 160;
    public const int DefaultMinHeight = 120;

    private static readonly Dictionary<string, (int W, int H)> KnownTypeSizes = new(StringComparer.Ordinal)
    {
        ["DropDown"] = (160, 40),
        ["DropDownMenu"] = (160, 40),
        ["DatasourceView"] = (320, 320),
        ["TableView"] = (360, 360),
        ["ChartView"] = (400, 300),
        ["LineChartView"] = (400, 300),
        ["BarChartView"] = (400, 300),
        ["ColumnChartView"] = (400, 300),
        ["CoreChartView"] = (400, 300),
        ["CardView"] = (320, 320),
        ["ExpansionView"] = (320, 320),
        ["ListView"] = (320, 320),
        ["ColumnsView"] = (320, 320),
        ["TreeControlView"] = (320, 320),
        ["CollectionsView"] = (320, 320),
        ["ScrollView"] = (200, 200),
        ["ImageView"] = (200, 160),
        ["ImageSliderView"] = (320, 200),
        ["HighchartView"] = (400, 300),
        ["HighchartGenericView"] = (400, 300),
        ["SearchView"] = (240, 60),
        ["ComboboxView"] = (200, 50),
        ["CheckboxView"] = (160, 50),
        ["TextBoxView"] = (200, 50),
        ["MultiLineTextView"] = (280, 120),
        ["EditView"] = (320, 240),
        ["FormInputView"] = (360, 320),
        ["RepositoryGridView"] = (480, 320),
        ["DataTableView"] = (360, 280),
        ["VideoView"] = (320, 240),
        ["CameraView"] = (320, 240),
        ["CameraStreamView"] = (320, 320),
        ["AudioView"] = (280, 80),
        ["ButtonView"] = (120, 40),
        ["TextView"] = (120, 32),
        ["StatusView"] = (200, 32),
        ["ToolsView"] = (200, 40),
        ["MenuView"] = (200, 40),
        ["BreadcrumbView"] = (320, 40),
        ["PaginationView"] = (320, 44),
        ["MasterPage"] = (640, 480),
        ["PageItem"] = (640, 480),
        ["TabsView"] = (320, 240),
        ["FlexContainer"] = (320, 240),
        ["AppViewBase"] = (DefaultMinWidth, DefaultMinHeight),
    };

    public static void EnsureVisibleInDesigner(AppViewBase view)
    {
        if (view is null)
            return;

        var (hintW, hintH) = GetTypeSizeHints(view);
        ApplyMinimums(view, hintW, hintH);

        Control ctrl;
        try
        {
            ctrl = view.control as Control ?? view.ToControl();
        }
        catch
        {
            NormalizeViewSize(view, hintW, hintH);
            return;
        }

        if (ctrl is null)
        {
            NormalizeViewSize(view, hintW, hintH);
            return;
        }

        void SyncFromControl()
        {
            if (ctrl.IsDisposed)
                return;

            var measuredW = Measure(ctrl.Width, ctrl.MinimumSize.Width, ctrl.PreferredSize.Width, hintW, view.MinWidth);
            var measuredH = Measure(ctrl.Height, ctrl.MinimumSize.Height, ctrl.PreferredSize.Height, hintH, view.MinHeight);

            NormalizeViewSize(view, measuredW, measuredH);

            if (view.control is Control bound && !bound.IsDisposed)
            {
                bound.MinimumSize = new Size(view.MinWidth, view.MinHeight);
                if (bound.Width < view.Width)
                    bound.Width = view.Width;
                if (bound.Height < view.Height)
                    bound.Height = view.Height;
            }
        }

        if (ctrl.IsHandleCreated)
            SyncFromControl();
        else
        {
            ctrl.HandleCreated += (_, _) => SyncFromControl();
            SyncFromControl();
        }
    }

    private static int Measure(int actual, int minimum, int preferred, int hint, int viewMinimum)
    {
        var size = Math.Max(actual, minimum);
        if (size < MinVisibleSize)
            size = Math.Max(size, preferred);
        if (size < MinVisibleSize)
            size = Math.Max(size, hint);
        if (size < MinVisibleSize)
            size = Math.Max(size, viewMinimum);
        return Math.Max(size, MinVisibleSize);
    }

    private static void ApplyMinimums(AppViewBase view, int hintW, int hintH)
    {
        if (view.MinWidth < MinVisibleSize)
            view.MinWidth = Math.Max(view.MinWidth, hintW);
        if (view.MinHeight < MinVisibleSize)
            view.MinHeight = Math.Max(view.MinHeight, hintH);
    }

    private static void NormalizeViewSize(AppViewBase view, int minW, int minH)
    {
        view.MinWidth = Math.Max(view.MinWidth, minW);
        view.MinHeight = Math.Max(view.MinHeight, minH);
        if (view.Width < view.MinWidth)
            view.Width = view.MinWidth;
        if (view.Height < view.MinHeight)
            view.Height = view.MinHeight;
    }

    private static (int W, int H) GetTypeSizeHints(AppViewBase view)
    {
        for (var type = view.GetType(); type is not null; type = type.BaseType)
        {
            if (KnownTypeSizes.TryGetValue(type.Name, out var size))
                return size;
            if (typeof(HighchartView).IsAssignableFrom(type))
                return (400, 300);
        }

        return (DefaultMinWidth, DefaultMinHeight);
    }
}
