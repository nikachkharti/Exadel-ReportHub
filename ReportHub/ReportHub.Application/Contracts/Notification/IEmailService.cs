namespace ReportHub.Application.Contracts.Notification
{
    public interface IEmailService
    {
        Task Send(string to, string subject, string body);
        Task Send(string to, string subject, string body, string filePath = null);
    }
}
