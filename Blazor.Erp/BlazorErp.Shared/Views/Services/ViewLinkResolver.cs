using BlazorErp.Shared.Views.Abstractions;
using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.Views.Services;

public sealed class ViewLinkResolver(IViewRegistry registry) : IViewLinkResolver
{
    public bool TryResolve(ViewDynamicLink link, out ViewDescriptor descriptor)
    {
        if (string.IsNullOrWhiteSpace(link.Key))
        {
            descriptor = new ViewDescriptor();
            return false;
        }

        return registry.TryGet(link.Key, out descriptor!);
    }
}
