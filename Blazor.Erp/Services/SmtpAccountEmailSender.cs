using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BlazorErp.Services;

public sealed class SmtpAccountEmailSender(
    IOptions<SmtpOptions> options,
    ILogger<SmtpAccountEmailSender> logger) : IAccountEmailSender
{
    public async Task SendPasswordResetLinkAsync(string email, string subject, string htmlMessage)
    {
        var smtp = options.Value;
        if (!smtp.IsConfigured)
        {
            logger.LogWarning(
                "SMTP не настроен (секция Smtp в appsettings). Письмо для {Email} не отправлено. Subject: {Subject}",
                email,
                subject);
            return;
        }

        var fromAddress = smtp.FromAddress.Trim();
        var password = smtp.Password.Trim();

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(smtp.Host, smtp.Port, ResolveSecureSocketOptions(smtp.Port));

            if (client.AuthenticationMechanisms.Contains("XOAUTH2"))
                client.AuthenticationMechanisms.Remove("XOAUTH2");

            await client.AuthenticateAsync(fromAddress, password);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtp.FromName, fromAddress));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            logger.LogInformation("Password reset email sent to {Email}", email);
        }
        catch (AuthenticationException ex)
        {
            logger.LogError(ex, "SMTP authentication failed when sending to {Email}", email);
            throw new InvalidOperationException(
                "Сервер почты отклонил вход. Проверьте Smtp:FromAddress и Smtp:Password в appsettings.",
                ex);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            logger.LogError(ex, "Failed to send password reset email to {Email}", email);
            throw new InvalidOperationException("Не удалось отправить сообщение по электронной почте", ex);
        }
    }

    private static SecureSocketOptions ResolveSecureSocketOptions(int port) => port switch
    {
        465 => SecureSocketOptions.SslOnConnect,
        587 => SecureSocketOptions.StartTls,
        _ => SecureSocketOptions.Auto
    };
}
