using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.ReportSchedule.DTOs
{
    public record ReportScheduleForGettingDto
    (
        string Id,
        string CustomerId,
        string CronExpression,
        ReportFormat Format,
        bool IsActive
    );
}
