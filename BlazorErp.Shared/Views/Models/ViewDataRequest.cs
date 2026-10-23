namespace BlazorErp.Shared.Views.Models;

public sealed class ViewDataRequest
{
    public ViewDescriptor Descriptor { get; set; } = new();
    public ViewDynamicLink Link { get; set; } = new();
    public IReadOnlyDictionary<string, object?> Context { get; set; } = new Dictionary<string, object?>();
}
