using MediatR;
using ReportHub.Application.Features.ReportSchedule.DTOs;

namespace ReportHub.Application.Features.ReportSchedule.Queries
{
    public record GetReportScheduleByIdQuery(string Id) : IRequest<ReportScheduleForGettingDto>;
}
