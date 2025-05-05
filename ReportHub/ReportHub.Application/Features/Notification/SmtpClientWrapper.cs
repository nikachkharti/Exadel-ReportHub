using MailKit.Net.Smtp;
using MimeKit;
using ReportHub.Application.Contracts.Notification;

namespace ReportHub.Application.Features.Notification
{
    public class SmtpClientWrapper : ISmtpClientWrapper
    {
        private readonly SmtpClient _smtpClient = new();

        public Task ConnectAsync(string host, int port, bool useSsl) =>
            _smtpClient.ConnectAsync(host, port, useSsl);

        public Task AuthenticateAsync(string username, string password) =>
            _smtpClient.AuthenticateAsync(username, password);

        public Task SendAsync(MimeMessage message) =>
            _smtpClient.SendAsync(message);

        public Task DisconnectAsync(bool quit) =>
            _smtpClient.DisconnectAsync(quit);

        public void Dispose() => _smtpClient.Dispose();
    }

}
