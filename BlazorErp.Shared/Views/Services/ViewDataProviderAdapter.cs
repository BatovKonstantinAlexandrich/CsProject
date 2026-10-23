using BlazorErp.Shared.Views.Abstractions;
using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.Views.Services;

/// <summary>Базовый адаптер для типизированных провайдеров данных представлений.</summary>
public abstract class ViewDataProviderAdapter : IViewDataProvider
{
    public abstract string ProviderKey { get; }
    public abstract ViewKind Kind { get; }

    public virtual bool CanProvide(ViewDescriptor descriptor) =>
        descriptor.Kind == Kind &&
        (string.IsNullOrWhiteSpace(descriptor.DataProviderKey) ||
         string.Equals(descriptor.DataProviderKey, ProviderKey, StringComparison.OrdinalIgnoreCase));

    public abstract Task<object?> LoadAsync(ViewDataRequest request, CancellationToken cancellationToken = default);
}

public abstract class TableViewDataProviderBase : ViewDataProviderAdapter, ITableViewDataProvider
{
    public override ViewKind Kind => ViewKind.Table;

    public async Task<ViewTableData> LoadTableAsync(ViewDataRequest request, CancellationToken cancellationToken = default) =>
        await LoadTableCoreAsync(request, cancellationToken);

    public override async Task<object?> LoadAsync(ViewDataRequest request, CancellationToken cancellationToken = default) =>
        await LoadTableAsync(request, cancellationToken);

    protected abstract Task<ViewTableData> LoadTableCoreAsync(ViewDataRequest request, CancellationToken cancellationToken);
}

public abstract class ListViewDataProviderBase : ViewDataProviderAdapter, IListViewDataProvider
{
    public override ViewKind Kind => ViewKind.List;

    public async Task<ViewListData> LoadListAsync(ViewDataRequest request, CancellationToken cancellationToken = default) =>
        await LoadListCoreAsync(request, cancellationToken);

    public override async Task<object?> LoadAsync(ViewDataRequest request, CancellationToken cancellationToken = default) =>
        await LoadListAsync(request, cancellationToken);

    protected abstract Task<ViewListData> LoadListCoreAsync(ViewDataRequest request, CancellationToken cancellationToken);
}

public abstract class HierarchyViewDataProviderBase : ViewDataProviderAdapter, IHierarchyViewDataProvider
{
    public override ViewKind Kind => ViewKind.Hierarchy;

    public async Task<ViewHierarchyData> LoadHierarchyAsync(ViewDataRequest request, CancellationToken cancellationToken = default) =>
        await LoadHierarchyCoreAsync(request, cancellationToken);

    public override async Task<object?> LoadAsync(ViewDataRequest request, CancellationToken cancellationToken = default) =>
        await LoadHierarchyAsync(request, cancellationToken);

    protected abstract Task<ViewHierarchyData> LoadHierarchyCoreAsync(ViewDataRequest request, CancellationToken cancellationToken);
}

public abstract class ChartViewDataProviderBase : ViewDataProviderAdapter, IChartViewDataProvider
{
    public override ViewKind Kind => ViewKind.Chart;

    public async Task<ViewChartData> LoadChartAsync(ViewDataRequest request, CancellationToken cancellationToken = default) =>
        await LoadChartCoreAsync(request, cancellationToken);

    public override async Task<object?> LoadAsync(ViewDataRequest request, CancellationToken cancellationToken = default) =>
        await LoadChartAsync(request, cancellationToken);

    protected abstract Task<ViewChartData> LoadChartCoreAsync(ViewDataRequest request, CancellationToken cancellationToken);
}

public abstract class CardViewDataProviderBase : ViewDataProviderAdapter, ICardViewDataProvider
{
    public override ViewKind Kind => ViewKind.Card;

    public async Task<ViewCardData> LoadCardAsync(ViewDataRequest request, CancellationToken cancellationToken = default) =>
        await LoadCardCoreAsync(request, cancellationToken);

    public override async Task<object?> LoadAsync(ViewDataRequest request, CancellationToken cancellationToken = default) =>
        await LoadCardAsync(request, cancellationToken);

    protected abstract Task<ViewCardData> LoadCardCoreAsync(ViewDataRequest request, CancellationToken cancellationToken);
}
