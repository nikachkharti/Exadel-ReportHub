using MediatR;
using ReportHub.Application.Features.Customers.DTOs;

namespace ReportHub.Application.Features.Customers.Queries
{
    public record GetAllCustomersQuery
    (
        string clientId,
        int? PageNumber,
        int? PageSize,
        string SortingParameter,
        bool Ascending = false
    ) : IRequest<IEnumerable<CustomerForGettingDto>>;
}