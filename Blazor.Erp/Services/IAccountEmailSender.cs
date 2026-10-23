namespace BlazorErp.Services;

public interface IAccountEmailSender
{
    Task SendPasswordResetLinkAsync(string email, string subject, string htmlMessage);
}
