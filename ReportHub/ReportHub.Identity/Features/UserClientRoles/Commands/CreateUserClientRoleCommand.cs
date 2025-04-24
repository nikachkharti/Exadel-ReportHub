using MediatR;

namespace ReportHub.Identity.Features.UserClientRoles.Commands;

public record CreateUserClientRoleCommand(string UserId, string RoleId, string ClientId = "System") : IRequest<string>;
