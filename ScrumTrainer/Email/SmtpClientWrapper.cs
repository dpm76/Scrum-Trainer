using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace ScrumTrainer.Email;

public class SmtpClientWrapper(IOptions<EmailSettings> settings) : ISmtpClient
{
    private readonly EmailSettings _settings = settings.Value;

    public async Task SendMailAsync(MailMessage message)
    {
        using var client = 
            new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true
            };

        await client.SendMailAsync(message);
    }
}
