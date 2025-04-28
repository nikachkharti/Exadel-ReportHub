using MediatR;
using System.Security.Claims;

namespace ReportHub.Identity.Application.Features.Auth.Commands;

public record RefreshTokenCommand(ClaimsPrincipal Principal) : IRequest<ClaimsPrincipal>;
