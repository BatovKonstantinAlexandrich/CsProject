using System.Collections.Concurrent;
using BlazorErp.Shared.Views.Abstractions;
using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.Views.Services;

public sealed class ViewRegistry : IViewRegistry
{
    private readonly ConcurrentDictionary<string, ViewDescriptor> _descriptors = new(StringComparer.OrdinalIgnoreCase);

    public void Register(ViewDescriptor descriptor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(descriptor.Key);
        _descriptors[descriptor.Key] = descriptor;
    }

    public bool TryGet(string key, out ViewDescriptor descriptor) =>
        _descriptors.TryGetValue(key, out descriptor!);

    public IReadOnlyCollection<ViewDescriptor> GetAll() => _descriptors.Values.ToList();
}
