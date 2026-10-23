using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.Views.Abstractions;

public interface ITableViewDataProvider : IViewDataProvider
{
    Task<ViewTableData> LoadTableAsync(ViewDataRequest request, CancellationToken cancellationToken = default);
}
