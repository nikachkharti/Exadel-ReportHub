using MediatR;

namespace ReportHub.Application.Features.CLientUsers.Commands;

public record DeleteClientUserCommand(string ClientId, string UserId) : IRequest<string>;

