using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ScrumTrainer.Data;

namespace ScrumTrainer.Email;

public class EmailSender(IOptions<EmailSettings> settings, ISmtpClient smtpClient) : IEmailSender<ApplicationUser>
{
    private readonly EmailSettings _settings = settings.Value;
    private readonly ISmtpClient _smtpClient = smtpClient;

    public EmailSender(IOptions<EmailSettings> settings): this(settings, new SmtpClientWrapper(settings))
    {
    }

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

        var mail = new MailMessage
        {
            From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        mail.To.Add(email);

        await _smtpClient.SendMailAsync(mail);
    }
}