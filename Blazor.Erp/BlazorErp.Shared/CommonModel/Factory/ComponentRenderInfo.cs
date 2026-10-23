namespace BlazorErp.Shared.CommonModel.Factory;

public sealed class ComponentRenderInfo
{
    public required Type ComponentType { get; init; }
    public Dictionary<string, object?> Parameters { get; init; } = new(StringComparer.OrdinalIgnoreCase);
}
