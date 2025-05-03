using MimeKit;

namespace ReportHub.Application.Contracts.Notification
{
    public interface ISmtpClientWrapper : IDisposable
    {
        Task ConnectAsync(string host, int port, bool useSsl);
        Task AuthenticateAsync(string username, string password);
        Task SendAsync(MimeMessage message);
        Task DisconnectAsync(bool quit);
    }
}
