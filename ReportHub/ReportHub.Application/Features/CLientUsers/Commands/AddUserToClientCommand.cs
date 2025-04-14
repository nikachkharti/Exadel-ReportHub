using MediatR;

namespace ReportHub.Application.Features.CLientUsers.Commands;

public record AddUserToClientCommand(string ClientId, string UserId, string Role) : IRequest<bool>;
