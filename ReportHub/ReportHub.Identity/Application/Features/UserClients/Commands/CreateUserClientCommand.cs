using MediatR;

namespace ReportHub.Identity.Application.Features.UserClients.Commands;

public record CreateUserClientCommand(string UserId, string Role, string ClientId = "System") : IRequest<string>;
