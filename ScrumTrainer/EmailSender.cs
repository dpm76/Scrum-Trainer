using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ScrumTrainer.Data;

namespace ScrumTrainer;

public class EmailSender(IOptions<EmailSettings> settings) : IEmailSender<ApplicationUser>
{
    private readonly EmailSettings _settings = settings.Value;

    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        => await SendEmailAsync(user.Email ?? string.Empty, "Confirmation Link", confirmationLink);

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        => await SendEmailAsync(user.Email ?? string.Empty, "Password Reset Code", resetCode);

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        => await SendEmailAsync(user.Email ?? string.Empty, "Password Reset Link", resetLink);

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = true
        };

        var mail = new MailMessage
        {
            From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        mail.To.Add(email);

        await client.SendMailAsync(mail);
    }
}