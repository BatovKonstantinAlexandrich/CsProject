using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.Views.Abstractions;

public interface IViewRegistry
{
    void Register(ViewDescriptor descriptor);
    bool TryGet(string key, out ViewDescriptor descriptor);
    IReadOnlyCollection<ViewDescriptor> GetAll();
}
