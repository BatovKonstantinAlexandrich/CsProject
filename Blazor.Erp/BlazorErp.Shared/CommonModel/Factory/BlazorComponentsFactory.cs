using BlazorErp.Shared.CommonModel.Components;
using BlazorErp.Shared.CommonModel.ViewCore;
using BlazorErp.Shared.CommonModel.Views;
using BlazorErp.Shared.Views.Components;
using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.CommonModel.Factory;

public sealed class BlazorComponentsFactory : IComponentsFactory
{
    private readonly Dictionary<string, string> _names = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(TableAppView)] = nameof(TableView),
        [nameof(ListAppView)] = nameof(ListView),
        [nameof(HierarchyAppView)] = nameof(HierarchyView),
        [nameof(ChartAppView)] = nameof(ChartView),
        [nameof(CardAppView)] = nameof(CardView),
        [nameof(DialogAppView)] = nameof(ViewDialog),
        [nameof(NavigationDrawerAppView)] = nameof(ViewNavigationDrawer),
        [nameof(AppViewBase)] = nameof(SharedComponents),
        [nameof(PanelAppView)] = nameof(SharedComponents),
    };

    public void Bind(BaseComponent component, ViewItem view) => Bind(view, component);

    public void Bind(ViewItem view, BaseComponent component)
    {
        component.ViewItem = view;
        if (view is AppViewBase appView)
            appView.Component = component;
    }

    public void BindEvents(BaseComponent component, ViewItem view)
    {
        view.DataState["__eventsBound"] = true;
    }

    public void BindEvents(ViewItem view, BaseComponent component) => BindEvents(component, view);

    public void BindSideControls(AppViewBase view, BaseComponent component)
    {
        Bind(component, view);
        foreach (var side in view.EnumerateLayoutAndControls())
            Bind(component, side);
    }

    public ComponentRenderInfo CreateControl(ViewItem view) =>
        view switch
        {
            TableAppView table => CreateTable(table),
            ListAppView list => CreateList(list),
            HierarchyAppView hierarchy => CreateTree(hierarchy),
            ChartAppView chart => CreateChart(chart),
            CardAppView card => CreateCard(card),
            DialogAppView dialog => CreateDialog(dialog),
            NavigationDrawerAppView drawer => CreateNavigationDrawer(drawer),
            AppViewBase app => CreateViewPanel(app),
            _ => CreateViewPanel(WrapUnknown(view))
        };

    public ComponentRenderInfo CreateControl(string name, ViewItem view)
    {
        if (string.Equals(name, nameof(TableView), StringComparison.OrdinalIgnoreCase))
            return CreateTable(WrapAs<TableAppView>(view, ViewKind.Table));
        if (string.Equals(name, nameof(ListView), StringComparison.OrdinalIgnoreCase))
            return CreateList(WrapAs<ListAppView>(view, ViewKind.List));
        if (string.Equals(name, nameof(HierarchyView), StringComparison.OrdinalIgnoreCase))
            return CreateTree(WrapAs<HierarchyAppView>(view, ViewKind.Hierarchy));
        if (string.Equals(name, nameof(ChartView), StringComparison.OrdinalIgnoreCase))
            return CreateChart(WrapAs<ChartAppView>(view, ViewKind.Chart));
        if (string.Equals(name, nameof(CardView), StringComparison.OrdinalIgnoreCase))
            return CreateCard(WrapAs<CardAppView>(view, ViewKind.Card));

        return CreateControl(view);
    }

    public ComponentRenderInfo CreatePanel(AppViewBase view) => CreateViewPanel(view);

    public ComponentRenderInfo CreateTable(TableAppView view) =>
        new()
        {
            ComponentType = typeof(TableView),
            Parameters = new Dictionary<string, object?>
            {
                [nameof(TableView.Data)] = view.TableData ?? new ViewTableData { Title = view.Title }
            }
        };

    public ComponentRenderInfo CreateList(ListAppView view) =>
        new()
        {
            ComponentType = typeof(ListView),
            Parameters = new Dictionary<string, object?>
            {
                [nameof(ListView.Data)] = view.ListData ?? new ViewListData { Title = view.Title }
            }
        };

    public ComponentRenderInfo CreateTree(HierarchyAppView view) =>
        new()
        {
            ComponentType = typeof(HierarchyView),
            Parameters = new Dictionary<string, object?>
            {
                [nameof(HierarchyView.Data)] = view.HierarchyData ?? new ViewHierarchyData { Title = view.Title }
            }
        };

    public ComponentRenderInfo CreateChart(ChartAppView view) =>
        new()
        {
            ComponentType = typeof(ChartView),
            Parameters = new Dictionary<string, object?>
            {
                [nameof(ChartView.Data)] = view.ChartData ?? new ViewChartData { Title = view.Title }
            }
        };

    public ComponentRenderInfo CreateCard(CardAppView view) =>
        new()
        {
            ComponentType = typeof(CardView),
            Parameters = new Dictionary<string, object?>
            {
                [nameof(CardView.Data)] = view.CardData ?? new ViewCardData { Title = view.Title }
            }
        };

    public ComponentRenderInfo CreateViewPanel(AppViewBase view) =>
        new()
        {
            ComponentType = typeof(SharedComponents),
            Parameters = new Dictionary<string, object?>
            {
                [nameof(SharedComponents.RootView)] = view
            }
        };

    public Dictionary<string, string> GetComponentNames() => new(_names);

    private ComponentRenderInfo CreateDialog(DialogAppView view) =>
        new()
        {
            ComponentType = typeof(ViewDialog),
            Parameters = new Dictionary<string, object?>
            {
                [nameof(ViewDialog.Title)] = view.Title,
                [nameof(ViewDialog.EmbeddedLink)] = view.DynamicLink
            }
        };

    private ComponentRenderInfo CreateNavigationDrawer(NavigationDrawerAppView view) =>
        new()
        {
            ComponentType = typeof(ViewNavigationDrawer),
            Parameters = new Dictionary<string, object?>
            {
                [nameof(ViewNavigationDrawer.Title)] = view.Title,
                [nameof(ViewNavigationDrawer.Items)] = view.NavItems
            }
        };

    private static AppViewBase WrapUnknown(ViewItem view)
    {
        var panel = new PanelAppView { Title = view.Title };
        panel.Append(view);
        return panel;
    }

    private static T WrapAs<T>(ViewItem source, ViewKind kind) where T : AppViewBase, new()
    {
        var view = new T
        {
            Title = source.Title,
            ViewKind = kind,
            Payload = (source as AppViewBase)?.Payload
        };
        foreach (var child in source.Controls)
            view.Append(child);
        return view;
    }
}
