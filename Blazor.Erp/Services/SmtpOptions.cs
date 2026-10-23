namespace BlazorErp.Services;

public class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string FromName { get; set; } = "Администрация сайта";
    public string FromAddress { get; set; } = "";
    public string Password { get; set; } = "";
    public string Host { get; set; } = "smtp.mail.ru";
    public int Port { get; set; } = 587;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(FromAddress) &&
        !string.IsNullOrWhiteSpace(Password) &&
        !string.IsNullOrWhiteSpace(Host);
}
