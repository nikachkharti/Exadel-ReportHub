using MediatR;

namespace ReportHub.Identity.Features.UserClients.Commands;

public record CreateUserClientCommand(string UserId, string Role, string ClientId = "System") : IRequest<string>;
