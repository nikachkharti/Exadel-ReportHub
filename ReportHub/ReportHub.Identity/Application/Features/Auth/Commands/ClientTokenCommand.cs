using MediatR;
using System.Security.Claims;

namespace ReportHub.Identity.Application.Features.Auth.Commands;

public record ClientTokenCommand(string UserClientId) : IRequest<ClaimsPrincipal>;