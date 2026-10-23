using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.Views.Abstractions;

public interface IChartViewDataProvider : IViewDataProvider
{
    Task<ViewChartData> LoadChartAsync(ViewDataRequest request, CancellationToken cancellationToken = default);
}
