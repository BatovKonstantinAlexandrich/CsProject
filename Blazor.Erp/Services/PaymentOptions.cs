namespace BlazorErp.Services;

public sealed class PaymentOptions
{
    public const string SectionName = "Payment";

    public string BaseUrl { get; set; } = "https://localhost:5001";
    public string DefaultPaymentSystemName { get; set; } = "СБП / Онлайн";
    public bool SimulateSms { get; set; } = true;
}
