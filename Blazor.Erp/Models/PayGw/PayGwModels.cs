using BlazorErp.Models.Enums;

namespace BlazorErp.Models.PayGw;

public class LegalEntitySource
{
    public int SourceId { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public string? Type { get; set; }
    public bool Active { get; set; } = true;
    public bool Splitting { get; set; }
}

public class CheckoutPaymentOption
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PaymentMethod Method { get; set; }
    public int? SourceId { get; set; }
}

public class PaymentLinkResult
{
    public string? PaymentUrl { get; set; }
    public string? Message { get; set; }
}
