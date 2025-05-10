using MediatR;
using ReportHub.Application.Features.Customers.DTOs;

namespace ReportHub.Application.Features.Customers.Queries
{
    public record GetAllCustomersQuery
    (
        int? PageNumber,
        int? PageSize,
        string SortingParameter,
        bool Ascending = false,
        CancellationToken CancellationToken = default
    ) : IRequest<IEnumerable<CustomerForGettingDto>>;
}