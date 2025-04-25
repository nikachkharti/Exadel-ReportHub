using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using ReportHub.Identity.Application.Features.Auth.Commands;
using ReportHub.Identity.Domain.Entities;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ReportHub.Identity.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly UserManager<User> _userManager;

    private readonly IMediator _mediator;

    public AuthController(
        IOpenIddictApplicationManager applicationManager,
        UserManager<User> userManager,
        IMediator mediator)
    {
        _applicationManager = applicationManager;
        _userManager = userManager;
        _mediator = mediator;
    }

    [HttpPost("/auth/login"), Produces("application/json")]
    public async Task<IActionResult> LoginAsAdmin()
    {
        OpenIddictRequest request = GetOpenIddictRequest();

        if (!request.IsClientCredentialsGrantType() && !request.IsPasswordGrantType())
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.InvalidGrant,
                ErrorDescription = "The specified grant type is not supported."
            });
        }

        var principal = await _mediator.Send(new LoginCommand(request.Username!, request.Password!));

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }


    [HttpPost("/auth/refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        OpenIddictRequest request = GetOpenIddictRequest();

        if (!request.IsRefreshTokenGrantType())
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.InvalidGrant,
                ErrorDescription = "The specified grant type is not supported."
            });
        }

        
        ClaimsPrincipal? principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
        if (principal is null)
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.InvalidGrant,
                ErrorDescription = "Invalid refresh token."
            });
        }

        var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType);


        identity
            .SetClaim(Claims.Subject, principal.GetClaim(Claims.Subject))
            .SetClaim(ClaimTypes.NameIdentifier, principal.GetClaim(ClaimTypes.NameIdentifier))
            .SetClaim(Claims.Audience, principal.GetClaim(Claims.Audience))
            .SetClaim(Claims.Email, principal.GetClaim(Claims.Email))
            .SetClaim(Claims.Name, principal.GetClaim(Claims.Name))
            .SetClaims(Claims.Role, [principal.GetClaim(Claims.Role) ?? "User"]);

        identity.SetDestinations(_ => [Destinations.AccessToken, "refresh_token"]);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<(User? user, bool isSuccess)> ValidateUserCreadentials(string userName, string password)
    {
        var user = await _userManager.FindByNameAsync(userName);

        var isSuccess = user is not null && await _userManager.CheckPasswordAsync(user, password);

        return (user, isSuccess);
    }

    private static void SetDestinations(ClaimsPrincipal principal)
    {
        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(Destinations.AccessToken, Destinations.IdentityToken);
        }
    }

    private void SetClaims(OpenIddictRequest request, User user, ClaimsIdentity identity, IList<string> roles)
    {
        identity
                    .SetClaim(Claims.Subject, request.ClientId)
                    .SetClaim(ClaimTypes.NameIdentifier, user.Id)
                    .SetClaim(Claims.Audience, "report-hub-api-audience")
                    .SetClaim(Claims.Email, user.Email)
                    .SetClaim(Claims.Name, user.UserName)
                    .SetClaims(Claims.Role, [.. roles]);
    }

    private static ClaimsIdentity GetIdentityClaims()
    {
        return new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);
    }

    private OpenIddictRequest GetOpenIddictRequest()
    {
        return HttpContext.GetOpenIddictServerRequest()
                      ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
    }
}