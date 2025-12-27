using System.Net.Mail;

namespace ScrumTrainer.Email;

public interface ISmtpClient
{
    Task SendMailAsync(MailMessage message);
}