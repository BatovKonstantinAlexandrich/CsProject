using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.Views.Abstractions;

public interface ICardViewDataProvider : IViewDataProvider
{
    Task<ViewCardData> LoadCardAsync(ViewDataRequest request, CancellationToken cancellationToken = default);
}
