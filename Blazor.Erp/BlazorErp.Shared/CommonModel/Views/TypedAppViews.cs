using BlazorErp.Shared.CommonModel.ViewCore;
using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.CommonModel.Views;

public sealed class TableAppView : AppViewBase
{
    public TableAppView()
    {
        ViewKind = ViewKind.Table;
    }

    public ViewTableData? TableData
    {
        get => Payload as ViewTableData;
        set => Payload = value;
    }
}

public sealed class ListAppView : AppViewBase
{
    public ListAppView()
    {
        ViewKind = ViewKind.List;
    }

    public ViewListData? ListData
    {
        get => Payload as ViewListData;
        set => Payload = value;
    }
}

public sealed class HierarchyAppView : AppViewBase
{
    public HierarchyAppView()
    {
        ViewKind = ViewKind.Hierarchy;
    }

    public ViewHierarchyData? HierarchyData
    {
        get => Payload as ViewHierarchyData;
        set => Payload = value;
    }
}

public sealed class ChartAppView : AppViewBase
{
    public ChartAppView()
    {
        ViewKind = ViewKind.Chart;
    }

    public ViewChartData? ChartData
    {
        get => Payload as ViewChartData;
        set => Payload = value;
    }
}

public sealed class CardAppView : AppViewBase
{
    public CardAppView()
    {
        ViewKind = ViewKind.Card;
    }

    public ViewCardData? CardData
    {
        get => Payload as ViewCardData;
        set => Payload = value;
    }
}

public sealed class PanelAppView : AppViewBase;

public sealed class DialogAppView : AppViewBase
{
    public DialogAppView()
    {
        ViewKind = ViewKind.Dialog;
    }
}

public sealed class NavigationDrawerAppView : AppViewBase
{
    public NavigationDrawerAppView()
    {
        ViewKind = ViewKind.NavigationDrawer;
    }

    public IList<ViewItemNode> NavItems { get; set; } = [];
}
