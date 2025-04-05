using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.IdentityModel.Tokens;
using ReportHub.Identity.Models;

namespace ReportHub.Identity.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly UserManager<User> _userManager;

    public AuthController(
        IOpenIddictApplicationManager applicationManager,
        UserManager<User> userManager)
    {
        _applicationManager = applicationManager;
        _userManager = userManager;
    }

    [HttpPost("~/connect/token"), Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ?? 
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
        
        if (request.ClientId is null)
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.InvalidClient,
                ErrorDescription = "The client_id parameter is missing."
            });
        }
        
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.InvalidGrant,
                ErrorDescription = "The user name or password is invalid."
            });
        }
        
        if (request.IsPasswordGrantType())
        {
            var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                              throw new InvalidOperationException("The application cannot be found.");

            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name,
                Claims.Role);
            
            identity.SetClaim(Claims.Subject, request.ClientId);
            identity.SetClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(application));
            identity.AddClaim(Claims.Audience, "report-hub-api-audience");

            identity.SetScopes(request.GetScopes());
            
            identity.SetDestinations(static claim => claim.Type switch
            {
                Claims.Name when claim.Subject.HasScope(Scopes.Profile)
                    => [Destinations.AccessToken, Destinations.IdentityToken],
                _ => [Destinations.AccessToken]
            });
            
            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        
        if (request.IsRefreshTokenGrantType())
        {
            var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            if (principal == null)
            {
                return BadRequest(new OpenIddictResponse
                {
                    Error = Errors.InvalidGrant,
                    ErrorDescription = "Invalid refresh token."
                });
            }

            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType);
            identity.SetClaim(Claims.Subject, principal.GetClaim(Claims.Subject));
            identity.AddClaim(Claims.Audience, "report-hub-api-audience");

            identity.SetDestinations(_ => [Destinations.AccessToken, "refresh_token"]);

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        
        return BadRequest(new OpenIddictResponse
        {
            Error = Errors.InvalidGrant,
            ErrorDescription = "The specified grant type is not supported."
        });
    }
}