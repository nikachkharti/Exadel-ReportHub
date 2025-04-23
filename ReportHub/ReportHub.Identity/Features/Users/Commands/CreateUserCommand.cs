using MediatR;

namespace ReportHub.Identity.Features.Users.Commands;

public record CreateUserCommand(string Username, string Email, string Password) : IRequest<string>;
