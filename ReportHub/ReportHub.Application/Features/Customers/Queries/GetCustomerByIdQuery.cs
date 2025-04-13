using MediatR;
using ReportHub.Application.Features.Customers.DTOs;

namespace ReportHub.Application.Features.Customers.Queries
{
    public record GetCustomerByIdQuery(string Id) : IRequest<CustomerForGettingDto>;
}
