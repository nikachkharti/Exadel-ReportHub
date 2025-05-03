using Microsoft.Extensions.Configuration;
using MimeKit.Text;
using MimeKit;
using ReportHub.Application.Contracts.Notification;
using MailKit.Net.Smtp;

namespace ReportHub.Application.Features.Notification
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        public async Task Send(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(configuration["EmailSettings:Sender"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                configuration["EmailSettings:SmtpServer"],
                int.Parse(configuration["EmailSettings:Port"]),
                bool.Parse(configuration["EmailSettings:UseSsl"])
            );
            await smtp.AuthenticateAsync(
                configuration["EmailSettings:Username"],
                configuration["EmailSettings:Password"]
            );
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
