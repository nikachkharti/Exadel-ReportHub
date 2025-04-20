using MediatR;

namespace ReportHub.Application.Features.Customers.Commands;

public record BaseCustomerCommand(string Name, string Email, string CountryId) : IRequest<string>;
