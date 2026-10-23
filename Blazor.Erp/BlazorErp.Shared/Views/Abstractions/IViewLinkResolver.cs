using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.Views.Abstractions;

public interface IViewLinkResolver
{
    bool TryResolve(ViewDynamicLink link, out ViewDescriptor descriptor);
}
