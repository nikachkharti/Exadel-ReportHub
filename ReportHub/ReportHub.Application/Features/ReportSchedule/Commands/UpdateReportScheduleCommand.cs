using MediatR;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.ReportSchedule.Commands
{
    public record UpdateReportScheduleCommand
    (
        string Id,
        string CustomerId,
        string CronExpression,
        ReportFormat Format,
        bool IsActive
    ) : IRequest<string>;
}
