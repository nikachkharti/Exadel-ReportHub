
namespace ReportHub.Application.Features.Customers.Commands;

public record UpdateCustomerCommand(string Id, string Name, string Email, string CountryId) : BaseCustomerCommand(Name, Email, CountryId);
