using System.ComponentModel.DataAnnotations;

namespace Console_CommonModel.CommonLib.LibModels;

/// <summary>
/// Зарегистрированный по URL WSDL сервис (хранилище ServiceBuilder).
/// </summary>
public class RegisteredWsdlService
{
    public int Id { get; set; }

    [Required]
    [MaxLength(2048)]
    public string WsdlUrl { get; set; }

    /// <summary>Краткое имя для дерева (по умолчанию — host + path).</summary>
    [MaxLength(512)]
    public string DisplayName { get; set; }

    public DateTime RegisteredAtUtc { get; set; }
}
