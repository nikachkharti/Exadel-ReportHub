using MediatR;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.ReportSchedule.Commands
{
    public record CreateReportScheduleCommand
    (
        string CustomerId,
        string CronExpression,
        ReportFormat Format,
        bool IsActive
    ) : IRequest<string>;
}
