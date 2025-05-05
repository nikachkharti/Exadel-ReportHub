using Microsoft.Extensions.Configuration;
using MimeKit;
using Moq;
using ReportHub.Application.Contracts.Notification;
using ReportHub.Application.Features.Notification;

namespace ReportHub.Tests.Application
{
    public class NotificationService_Should
    {
        public class EmailServiceTests
        {
            [Fact]
            public async Task Send_EmailIsComposedAndSentSuccessfully()
            {
                // Arrange
                var mockConfig = new Mock<IConfiguration>();
                mockConfig.Setup(c => c["EmailSettings:Sender"]).Returns("nika.chkhartishvili7@gmail.com");
                mockConfig.Setup(c => c["EmailSettings:SmtpServer"]).Returns("smtp.gmail.com");
                mockConfig.Setup(c => c["EmailSettings:Port"]).Returns("465");
                mockConfig.Setup(c => c["EmailSettings:UseSsl"]).Returns("true");
                mockConfig.Setup(c => c["EmailSettings:Username"]).Returns("nika.chkhartishvili7@gmail.com");
                mockConfig.Setup(c => c["EmailSettings:Password"]).Returns("iewu xuso begl sfba");

                var mockSmtpClient = new Mock<ISmtpClientWrapper>();

                var service = new EmailService(mockConfig.Object, mockSmtpClient.Object);

                // Act
                await service.Send("nika.chkhartishvili7@gmail.com", "Test Subject", "Test Body");

                // Assert
                mockSmtpClient.Verify(x => x.ConnectAsync("smtp.gmail.com", 465, true), Times.Once);
                mockSmtpClient.Verify(x => x.AuthenticateAsync("nika.chkhartishvili7@gmail.com", "iewu xuso begl sfba"), Times.Once);
                mockSmtpClient.Verify(x => x.SendAsync(It.Is<MimeMessage>(msg =>
                    msg.Subject == "Test Subject" &&
                    msg.TextBody == null &&
                    msg.HtmlBody == "Test Body" &&
                    msg.From.ToString().Contains("nika.chkhartishvili7@gmail.com") &&
                    msg.To.ToString().Contains("nika.chkhartishvili7@gmail.com")
                )), Times.Once);
                mockSmtpClient.Verify(x => x.DisconnectAsync(true), Times.Once);
            }
        }

    }
}
