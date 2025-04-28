using MediatR;
using System.Security.Claims;

namespace ReportHub.Identity.Application.Features.Auth.Commands;

public record LoginCommand(string UserName, string Password) : IRequest<ClaimsPrincipal>;