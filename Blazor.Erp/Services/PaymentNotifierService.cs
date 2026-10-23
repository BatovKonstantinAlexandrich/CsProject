using BlazorErp.Models.Enums;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BlazorErp.Services;

public sealed class PaymentNotifierService(
    IOptions<PaymentOptions> paymentOptions,
    IOptions<SmtpOptions> smtpOptions,
    ILogger<PaymentNotifierService> logger) : IPaymentNotifier
{
    public async Task<(bool Success, string? Error)> SendPaymentLinkAsync(
        PaymentLinkChannel channel,
        string destination,
        string paymentUrl,
        decimal amount,
        string orderReference)
    {
        return channel switch
        {
            PaymentLinkChannel.Sms => SendSms(destination, paymentUrl, amount, orderReference),
            PaymentLinkChannel.Email => await SendEmailAsync(destination, paymentUrl, amount, orderReference),
            _ => (false, "Неизвестный канал доставки")
        };
    }

    private (bool Success, string? Error) SendSms(
        string phone,
        string paymentUrl,
        decimal amount,
        string orderReference)
    {
        var normalized = PhoneNormalizer.Normalize(phone);
        if (normalized == null)
            return (false, "Некорректный номер телефона");

        var text = $"Оплата заказа {orderReference} на сумму {amount:N0} ₽: {paymentUrl}";

        if (paymentOptions.Value.SimulateSms)
        {
            logger.LogInformation("SMS (simulated) to {Phone}: {Text}", normalized, text);
            return (true, null);
        }

        logger.LogWarning("SMS gateway не настроен. Сообщение для {Phone} не отправлено.", normalized);
        return (false, "SMS-шлюз не настроен. Включите SimulateSms или подключите провайдера.");
    }

    private async Task<(bool Success, string? Error)> SendEmailAsync(
        string email,
        string paymentUrl,
        decimal amount,
        string orderReference)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return (false, "Некорректный e-mail");

        var smtp = smtpOptions.Value;
        if (!smtp.IsConfigured)
        {
            logger.LogWarning("SMTP не настроен. Письмо для {Email} не отправлено.", email);
            return (false, "SMTP не настроен (секция Smtp в appsettings).");
        }

        try
        {
            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(smtp.Host, smtp.Port, smtp.Port == 465
                ? MailKit.Security.SecureSocketOptions.SslOnConnect
                : MailKit.Security.SecureSocketOptions.StartTls);

            if (client.AuthenticationMechanisms.Contains("XOAUTH2"))
                client.AuthenticationMechanisms.Remove("XOAUTH2");

            await client.AuthenticateAsync(smtp.FromAddress.Trim(), smtp.Password.Trim());

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtp.FromName, smtp.FromAddress.Trim()));
            message.To.Add(MailboxAddress.Parse(email.Trim()));
            message.Subject = $"Ссылка на оплату заказа {orderReference}";
            message.Body = new BodyBuilder
            {
                HtmlBody = $"""
                    <p>Здравствуйте!</p>
                    <p>Для оплаты заказа <strong>{orderReference}</strong> на сумму <strong>{amount:N2} ₽</strong> перейдите по ссылке:</p>
                    <p><a href="{paymentUrl}">{paymentUrl}</a></p>
                    """
            }.ToMessageBody();

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            logger.LogInformation("Payment link email sent to {Email}", email);
            return (true, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send payment link email to {Email}", email);
            return (false, "Не удалось отправить письмо");
        }
    }
}

public static class PhoneNormalizer
{
    public static string? Normalize(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return null;

        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.Length == 11 && digits.StartsWith('8'))
            digits = "7" + digits[1..];
        if (digits.Length == 10)
            digits = "7" + digits;

        return digits.Length == 11 && digits.StartsWith('7') ? digits : null;
    }
}
