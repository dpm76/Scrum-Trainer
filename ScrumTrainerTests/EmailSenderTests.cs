using System.Net.Mail;
using Microsoft.Extensions.Options;
using ScrumTrainer.Data;
using ScrumTrainer.Email;

namespace ScrumTrainerTests;

public class EmailSenderTests
{
     private static EmailSender CreateSut(Mock<ISmtpClient> smtpMock)
    {
        var settings = Options.Create(new EmailSettings
        {
            SmtpServer = "mail.test.com",
            SmtpPort = 587,
            Username = "username",
            Password = "123456",
            SenderEmail = "sender@test.com",
            SenderName = "Test App"
        });

        return new EmailSender(settings, smtpMock.Object);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldSendEmail_WhenEmailIsValid()
    {
        // Arrange
        var smtpMock = new Mock<ISmtpClient>();

        var settings = Options.Create(new EmailSettings
        {
            SmtpServer = "mail.test.com",
            SmtpPort = 587,
            Username = "username",
            Password = "123456",
            SenderEmail = "sender@test.com",
            SenderName = "Test App"
        });

        var sut = new EmailSender(settings, smtpMock.Object);

        // Act
        await sut.SendEmailAsync("user@test.com", "Subject", "<p>Body</p>");

        // Assert
        smtpMock.Verify(
            x => x.SendMailAsync(It.Is<MailMessage>(m =>
                m.Subject == "Subject" &&
                m.IsBodyHtml &&
                m.To.Any(t => t.Address == "user@test.com")
            )),
            Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldNotSend_WhenEmailIsEmpty()
    {
        // Arrange
        var smtpMock = new Mock<ISmtpClient>();

        var settings = Options.Create(new EmailSettings
        {
            SmtpServer = "mail.test.com",
            SmtpPort = 587,
            Username = "username",
            Password = "123456",
            SenderEmail = "sender@test.com",
            SenderName = "Test App"
        });
        var sut = new EmailSender(settings, smtpMock.Object);

        // Act
        await sut.SendEmailAsync("", "Subject", "Body");

        // Assert
        smtpMock.Verify(x => x.SendMailAsync(It.IsAny<MailMessage>()), Times.Never);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldSendEmail_WithCorrectData()
    {
        // Arrange
        var smtpMock = new Mock<ISmtpClient>();
        var sut = CreateSut(smtpMock);

        // Act
        await sut.SendEmailAsync("user@test.com", "Subject", "<p>Body</p>");

        // Assert
        smtpMock.Verify(x =>
            x.SendMailAsync(It.Is<MailMessage>(m =>
                m.Subject == "Subject" &&
                m.Body == "<p>Body</p>" &&
                m.IsBodyHtml &&
                m.From!.Address == "sender@test.com" &&
                m.From.DisplayName == "Test App" &&
                m.To.Single().Address == "user@test.com"
            )),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task SendEmailAsync_ShouldNotSend_WhenEmailIsInvalid(string email)
    {
        // Arrange
        var smtpMock = new Mock<ISmtpClient>();
        var sut = CreateSut(smtpMock);

        // Act
        await sut.SendEmailAsync(email, "Subject", "Body");

        // Assert
        smtpMock.Verify(
            x => x.SendMailAsync(It.IsAny<MailMessage>()),
            Times.Never);
    }

    [Fact]
    public async Task SendConfirmationLinkAsync_ShouldSendConfirmationEmail()
    {
        // Arrange
        var smtpMock = new Mock<ISmtpClient>();
        var sut = CreateSut(smtpMock);

        var user = new ApplicationUser
        {
            Email = "user@test.com"
        };

        // Act
        await sut.SendConfirmationLinkAsync(user, user.Email, "confirmation-link");

        // Assert
        smtpMock.Verify(x =>
            x.SendMailAsync(It.Is<MailMessage>(m =>
                m.Subject == "Confirmation Link" &&
                m.Body == "confirmation-link"
            )),
            Times.Once);
    }

    [Fact]
    public async Task SendPasswordResetCodeAsync_ShouldSendResetCodeEmail()
    {
        // Arrange
        var smtpMock = new Mock<ISmtpClient>();
        var sut = CreateSut(smtpMock);

        var user = new ApplicationUser
        {
            Email = "user@test.com"
        };

        // Act
        await sut.SendPasswordResetCodeAsync(user, user.Email, "123456");

        // Assert
        smtpMock.Verify(x =>
            x.SendMailAsync(It.Is<MailMessage>(m =>
                m.Subject == "Password Reset Code" &&
                m.Body == "123456"
            )),
            Times.Once);
    }

    [Fact]
    public async Task SendPasswordResetLinkAsync_ShouldSendResetLinkEmail()
    {
        // Arrange
        var smtpMock = new Mock<ISmtpClient>();
        var sut = CreateSut(smtpMock);

        var user = new ApplicationUser
        {
            Email = "user@test.com"
        };

        // Act
        await sut.SendPasswordResetLinkAsync(user, user.Email!, "reset-link");

        // Assert
        smtpMock.Verify(x =>
            x.SendMailAsync(It.Is<MailMessage>(m =>
                m.Subject == "Password Reset Link" &&
                m.Body == "reset-link"
            )),
            Times.Once);
    }

}
