using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.Views.Abstractions;

public interface IListViewDataProvider : IViewDataProvider
{
    Task<ViewListData> LoadListAsync(ViewDataRequest request, CancellationToken cancellationToken = default);
}
