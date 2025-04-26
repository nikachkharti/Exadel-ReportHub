using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using ReportHub.Identity.Application.Features.Auth.Commands;
using ReportHub.Identity.Application.Interfaces.ServiceInterfaces;
using ReportHub.Identity.Application.Validators.Exceptions;
using ReportHub.Identity.Domain.Entities;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ReportHub.Identity.Application.Features.Auth.Handlers.CommandHandlers;

public class RefreshTokenCommandHandler : BaseAuthCommandHandler, IRequestHandler<RefreshTokenCommand, ClaimsPrincipal>
{
    private readonly UserManager<User> _userManager;
    private readonly IPrincipalService _principalService;

    public RefreshTokenCommandHandler(UserManager<User> userManager, IPrincipalService principalService)
    {
        _userManager = userManager;
        _principalService = principalService;
    }

    public async Task<ClaimsPrincipal> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        EnsurePrincipalsNotNull(request.Principal);

        var user = await GetUserIfExist(request.Principal);

        var principal = _principalService.GetClaimsPrincipal(
                                            user, 
                                            request.Principal.GetClaim("Client")!, 
                                            request.Principal.GetClaim(Claims.Role)!);

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
