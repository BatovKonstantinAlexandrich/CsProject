using BlazorErp.Shared.CommonModel.Components;
using BlazorErp.Shared.CommonModel.Factory;
using BlazorErp.Shared.Views.Models;

namespace BlazorErp.Shared.CommonModel.ViewCore;

/// <summary>Модель представления с интеграцией фабрики компонентов (аналог AppViewBase).</summary>
public class AppViewBase : ViewItem
{
    private readonly AppViewBaseModel _meta = new();

    public AppViewBaseModel Meta => _meta;

    public new string Title
    {
        get => _meta.Title;
        set => _meta.Title = value;
    }

    public string Description
    {
        get => _meta.Description;
        set => _meta.Description = value;
    }

    public ViewKind ViewKind { get; set; }

    public ViewDynamicLink? DynamicLink { get; set; }

    public string? DataProviderKey { get; set; }

    public ViewItem? Left { get => Get<ViewItem>(nameof(Left)); set => Set(nameof(Left), value); }
    public ViewItem? Right { get => Get<ViewItem>(nameof(Right)); set => Set(nameof(Right), value); }
    public ViewItem? Top { get => Get<ViewItem>(nameof(Top)); set => Set(nameof(Top), value); }
    public ViewItem? Bottom { get => Get<ViewItem>(nameof(Bottom)); set => Set(nameof(Bottom), value); }

    public int Status { get; set; } = 200;
    public string StatusMessage { get; set; } = string.Empty;

    /// <summary>Ссылка на связанный Blazor-компонент после Bind.</summary>
    public BaseComponent? Component { get; set; }

    public IComponentsFactory? ComponentsFactory { get; set; }

    public object? Payload { get; set; }

    public AppViewBase EnsureFactory(IComponentsFactory factory)
    {
        ComponentsFactory ??= factory;
        return this;
    }

    public IEnumerable<ViewItem> EnumerateLayoutAndControls()
    {
        if (Top != null) yield return Top;
        if (Left != null) yield return Left;
        if (Right != null) yield return Right;
        if (Bottom != null) yield return Bottom;
        foreach (var child in Controls)
            yield return child;
    }

    public static AppViewBase FromDescriptor(ViewDescriptor descriptor, ViewDynamicLink? link = null) =>
        new()
        {
            Title = descriptor.Title,
            Description = descriptor.Description ?? string.Empty,
            ViewKind = descriptor.Kind,
            DataProviderKey = descriptor.DataProviderKey,
            DynamicLink = link ?? ViewDynamicLink.Parse(descriptor.Key)
        };
}
