public interface IEmailService
{
    void Recieve();
    void SendEmail(string email, string subject, string message);
    //void SendEmail(string email, string subject, string message, TypeFile[] resources);
}