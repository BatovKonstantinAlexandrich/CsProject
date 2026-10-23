using BlazorErp.Shared.Views.Abstractions;
using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.Views.Services;

public sealed class ViewDataProviderResolver(IEnumerable<IViewDataProvider> providers) : IViewDataProviderResolver
{
    private readonly IReadOnlyList<IViewDataProvider> _providers = providers.ToList();

    public IViewDataProvider? Resolve(ViewDescriptor descriptor)
    {
        if (!string.IsNullOrWhiteSpace(descriptor.DataProviderKey))
        {
            var byKey = _providers.FirstOrDefault(p =>
                string.Equals(p.ProviderKey, descriptor.DataProviderKey, StringComparison.OrdinalIgnoreCase));
            if (byKey != null)
                return byKey;
        }

        return _providers.FirstOrDefault(p => p.Kind == descriptor.Kind && p.CanProvide(descriptor));
    }
}
