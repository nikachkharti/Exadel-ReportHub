using Microsoft.Extensions.Configuration;
using MimeKit.Text;
using MimeKit;
using ReportHub.Application.Contracts.Notification;

namespace ReportHub.Application.Features.Notification
{
    public class EmailService(IConfiguration configuration, ISmtpClientWrapper smtpClient) : IEmailService
    {
        public async Task Send(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(configuration["EmailSettings:Sender"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            await smtpClient.ConnectAsync(
                configuration["EmailSettings:SmtpServer"],
                int.Parse(configuration["EmailSettings:Port"]),
                bool.Parse(configuration["EmailSettings:UseSsl"])
            );
            await smtpClient.AuthenticateAsync(
                configuration["EmailSettings:Username"],
                configuration["EmailSettings:Password"]
            );
            await smtpClient.SendAsync(email);
            await smtpClient.DisconnectAsync(true);
        }


        public async Task Send(string to, string subject, string body, string filePath = null)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(configuration["EmailSettings:Sender"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder();

            if (!string.IsNullOrWhiteSpace(body))
                builder.HtmlBody = body;

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                builder.Attachments.Add(filePath);

            email.Body = builder.ToMessageBody();

            await smtpClient.ConnectAsync(
                configuration["EmailSettings:SmtpServer"],
                int.Parse(configuration["EmailSettings:Port"]),
                bool.Parse(configuration["EmailSettings:UseSsl"])
            );
            await smtpClient.AuthenticateAsync(
                configuration["EmailSettings:Username"],
                configuration["EmailSettings:Password"]
            );
            await smtpClient.SendAsync(email);
            await smtpClient.DisconnectAsync(true);
        }


    }
}
