using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.Views.Abstractions;

public interface IViewDataProviderResolver
{
    IViewDataProvider? Resolve(ViewDescriptor descriptor);
}
