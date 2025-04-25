using MediatR;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using ReportHub.Identity.Application.Features.Auth.Commands;
using ReportHub.Identity.Application.Validators.Exceptions;
using ReportHub.Identity.Domain.Entities;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ReportHub.Identity.Application.Features.Auth.Handlers.CommandHandlers;

public class LoginCommandHandler : BaseAuthCommandHandler, IRequestHandler<LoginCommand, ClaimsPrincipal>
{
    private readonly UserManager<User> _userManager;

    public LoginCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ClaimsPrincipal> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await GetIfUserExist(request.UserName, request.Password);

        var identityClaims = GetIdentityClaims();

        SetClaims(user, identityClaims);
        SetScopes(identityClaims);

        var principal = new ClaimsPrincipal(identityClaims);

        SetDestinations(principal);

        return principal;
    }

    private async Task<User> GetIfUserExist(string userName, string password)
    {
        var user = await _userManager.FindByNameAsync(userName);

        var isSuccess = user is not null && await _userManager.CheckPasswordAsync(user, password);

        if (!isSuccess)
        {
            throw new UnauthorizedException("Invalid username or password");
        }

        return user!;
    }

    private static void SetDestinations(ClaimsPrincipal principal)
    {
        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(Destinations.AccessToken, Destinations.IdentityToken);
        }
    }
}
