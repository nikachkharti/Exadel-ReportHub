using MediatR;

namespace ReportHub.Application.Features.Customers.Commands
{
    public record DeleteCustomerCommand(string Id) : IRequest<string>;
}
