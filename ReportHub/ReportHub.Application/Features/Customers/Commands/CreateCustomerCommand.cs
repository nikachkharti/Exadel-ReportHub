using MediatR;

namespace ReportHub.Application.Features.Customers.Commands;

public record CreateCustomerCommand(string Name, string Email, string CountryId, string ClientId) : IRequest<string>;
