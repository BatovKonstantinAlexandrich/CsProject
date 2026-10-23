using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.Views.Abstractions;

public interface IHierarchyViewDataProvider : IViewDataProvider
{
    Task<ViewHierarchyData> LoadHierarchyAsync(ViewDataRequest request, CancellationToken cancellationToken = default);
}
