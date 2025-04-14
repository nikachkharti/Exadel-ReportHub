
namespace ReportHub.Application.Features.Customers.Commands;

public record UpdateCustomerCommand(string Id, string Name, string Email, string Address) : BaseCustomerCommand(Name, Email, Address);
