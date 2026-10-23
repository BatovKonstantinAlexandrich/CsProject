namespace BlazorErp.Shared.Views.Models;

/// <summary>Описание представления, регистрируемого в реестре динамических ссылок.</summary>
public sealed class ViewDescriptor
{
    public string Key { get; set; } = string.Empty;
    public ViewKind Kind { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    /// <summary>Идентификатор или имя типа провайдера данных (опционально).</summary>
    public string? DataProviderKey { get; set; }
    public IReadOnlyDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
}
