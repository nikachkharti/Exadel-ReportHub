namespace ReportHub.Application.Contracts.Notification
{
    public interface IEmailService
    {
        Task Send(string to, string subject, string body);
    }
}
