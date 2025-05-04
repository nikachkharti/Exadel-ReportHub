using MediatR;

namespace ReportHub.Application.Features.ReportSchedule.Commands
{
    public record DeleteReportScheduleCommand(string Id) : IRequest<string>;
}
