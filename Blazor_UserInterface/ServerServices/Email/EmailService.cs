
 

using MailKit.Net.Pop3;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using System;
using System.IO;

/// <summary>
/// Сервис умеет отправлять электронную почту и считывать входящие письма
/// </summary>
public class EmailService:IEmailService
{
    public static int Counter = StartupApplication.AddWithOptions((services,configuration)=> {
        services.AddSingleton(typeof(EmailOptions),(provider)=> {
            var section = configuration.GetSection(nameof(EmailOptions));

            var result = section.Get<EmailOptions>();
            if (result == null)
            {
                throw new Exception("Дополните конфигурацию приложения appsettings.json\n" + new EmailOptions().ToJsonOnScreen());
            }
            return result;
        });
        services.AddSingleton<EmailService>();
        services.AddSingleton<IEmailService,EmailService>();
    });

    private readonly EmailOptions _options;

    public EmailService(EmailOptions options)
    {
        _options = options;
    }


    /// <summary>
    /// Отправка сообщения по электронной почте
    /// </summary> 
    public void SendEmail(string email, string subject, string message)
    {
        using (var smtp = new MailKit.Net.Smtp.SmtpClient())
        {
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
            
            smtp.Connect(_options.smtpHost, _options.smtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.emailAddress, _options.emailPassword);
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_options.emailName, _options.emailAddress));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
                
            smtp.Send(emailMessage);
            smtp.Disconnect(true);
        }
    }


    /// <summary>
    /// Отправка сообщения по электронной почте с прикреплёнными файлами
    /// </summary> 
    public void SendEmail(string email, string subject, string message, 
                            TypeFile[] resources)
    {
        using (var smtp = new MailKit.Net.Smtp.SmtpClient())
        {
            smtp.ServerCertificateValidationCallback = (s, c, h, e) =>
            {
                return true;
            };

            smtp.Connect(_options.smtpHost, _options.smtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.emailAddress, _options.emailPassword);
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_options.emailName, _options.emailAddress));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            var builder = new BodyBuilder();
                
            builder.TextBody = message;
            if (resources != null)
            {
                foreach(var resource in resources)
                {
                    System.IO.File.WriteAllBytes(resource.Name, resource.Data);
                    builder.Attachments.Add(resource.Name);
                }
            }                             
            emailMessage.Body = builder.ToMessageBody();
               
            smtp.Send(emailMessage);
            smtp.Disconnect(true);
        }
    }


    /// <summary>
    /// Получение входящих сообщений
    /// </summary>
    public void Recieve()
    {
        using (var client = new Pop3Client())
        {                
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            client.Connect(_options.popHost, _options.popPort);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate(_options.emailAddress, _options.emailAddress );
            for (int i = 0; i < client.Count; i++)
            {
                MimeMessage message = client.GetMessage(i);                    
            }
            client.Disconnect(true);
        }
    }
}
