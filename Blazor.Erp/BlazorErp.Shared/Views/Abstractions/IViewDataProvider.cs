using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.Views.Abstractions;

public interface IViewDataProvider
{
    string ProviderKey { get; }
    ViewKind Kind { get; }
    bool CanProvide(ViewDescriptor descriptor);
    Task<object?> LoadAsync(ViewDataRequest request, CancellationToken cancellationToken = default);
}
