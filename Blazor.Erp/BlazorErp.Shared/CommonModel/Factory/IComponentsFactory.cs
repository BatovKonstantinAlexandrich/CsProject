using BlazorErp.Shared.CommonModel.Components;
using BlazorErp.Shared.CommonModel.ViewCore;
using BlazorErp.Shared.CommonModel.Views;

namespace BlazorErp.Shared.CommonModel.Factory;

/// <summary>
/// Фабрика Blazor-компонентов для моделей представлений (адаптация IComponentsFactory из Console_CommonModel).
/// </summary>
public interface IComponentsFactory
{
    void Bind(BaseComponent component, ViewItem view);
    void Bind(ViewItem view, BaseComponent component);
    void BindEvents(BaseComponent component, ViewItem view);
    void BindEvents(ViewItem view, BaseComponent component);
    void BindSideControls(AppViewBase view, BaseComponent component);

    ComponentRenderInfo CreateControl(ViewItem view);
    ComponentRenderInfo CreateControl(string name, ViewItem view);
    ComponentRenderInfo CreatePanel(AppViewBase view);
    ComponentRenderInfo CreateTable(TableAppView view);
    ComponentRenderInfo CreateList(ListAppView view);
    ComponentRenderInfo CreateTree(HierarchyAppView view);
    ComponentRenderInfo CreateChart(ChartAppView view);
    ComponentRenderInfo CreateCard(CardAppView view);
    ComponentRenderInfo CreateViewPanel(AppViewBase view);

    Dictionary<string, string> GetComponentNames();
}
