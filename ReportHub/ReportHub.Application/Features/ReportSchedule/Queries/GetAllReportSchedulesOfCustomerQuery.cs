using MediatR;
using ReportHub.Application.Features.ReportSchedule.DTOs;

namespace ReportHub.Application.Features.ReportSchedule.Queries
{
    public record GetAllReportSchedulesOfCustomerQuery
    (
        string CustomerId,
         int? PageNumber,
         int? PageSize,
         string SortingParameter,
         bool Ascending = false,
         CancellationToken CancellationToken = default
    ) : IRequest<IEnumerable<ReportScheduleForGettingDto>>;
}
