namespace BlazorErp.Components.Pages.Cashier;

public record NewBasketDialogResult(string ClientName, string? Phone, string? Email);

public record PaymentLinkDialogResult(bool SendSms, string Phone, bool SendEmail, string Email);
