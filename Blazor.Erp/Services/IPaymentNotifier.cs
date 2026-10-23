using BlazorErp.Models.Enums;

namespace BlazorErp.Services;

public interface IPaymentNotifier
{
    Task<(bool Success, string? Error)> SendPaymentLinkAsync(
        PaymentLinkChannel channel,
        string destination,
        string paymentUrl,
        decimal amount,
        string orderReference);
}
