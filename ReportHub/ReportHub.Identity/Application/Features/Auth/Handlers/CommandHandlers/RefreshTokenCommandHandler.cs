using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using ReportHub.Identity.Application.Features.Auth.Commands;
using ReportHub.Identity.Application.Validators.Exceptions;
using ReportHub.Identity.Domain.Entities;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ReportHub.Identity.Application.Features.Auth.Handlers.CommandHandlers;

public class RefreshTokenCommandHandler : BaseAuthCommandHandler, IRequestHandler<RefreshTokenCommand, ClaimsPrincipal>
{
    private readonly UserManager<User> _userManager;

    public RefreshTokenCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ClaimsPrincipal> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        EnsurePrincipalsNotNull(request.Principal);

        var user = await GetUserIfExist(request.Principal);

        var identityClaim = GetIdentityClaims();

        
        SetClaims(user, identityClaim, request.Principal.GetClaim("Client")!, request.Principal.GetClaim(Claims.Role)!);
        SetScopes(identityClaim);

        identityClaim.SetDestinations(_ => [Destinations.AccessToken, "refresh_token"]);

        var principal = new ClaimsPrincipal(identityClaim);

        return principal;
    }

    private async Task<User> GetUserIfExist(ClaimsPrincipal principal)
    {
        var userId = principal.GetClaim(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(userId!);

        if (user is null)
        {
            throw new UnauthorizedException("Invalid token");
        }

        return user;
    }
}
