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
        var request = HttpContext.GetOpenIddictServerRequest() 
                      ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
        
        if (request.ClientId is null)
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.InvalidClient,
                ErrorDescription = "The client_id parameter is missing."
            });
        }
        
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user is null)
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.InvalidGrant,
                ErrorDescription = "The user name or password is invalid."
            });
        }
        
        var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isValidPassword)
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.InvalidGrant,
                ErrorDescription = "The user name or password is invalid."
            });
        }
        
        if (request.IsClientCredentialsGrantType() || request.IsPasswordGrantType())
        {
            if (await _applicationManager.FindByClientIdAsync(request.ClientId) is null)
                throw new InvalidOperationException("The application cannot be found.");
            
            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            var userRoles = await _userManager.GetRolesAsync(user);
            
            identity
                .SetClaim(Claims.Subject, request.ClientId)
                .SetClaim(Claims.Audience, "report-hub-api-audience")
                .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                .SetClaims(Claims.Role, [..userRoles]);
            
            identity.SetScopes(request.GetScopes());
            
            var principal = new ClaimsPrincipal(identity);
            
            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(Destinations.AccessToken, Destinations.IdentityToken);
            }

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        
        if (request.IsRefreshTokenGrantType())
        {
            var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
            if (principal is null)
            {
                return BadRequest(new OpenIddictResponse
                {
                    Error = Errors.InvalidGrant,
                    ErrorDescription = "Invalid refresh token."
                });
            }

            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType);
            identity.SetClaim(Claims.Subject, principal.GetClaim(Claims.Subject));
            identity.SetClaim(Claims.Audience, "report-hub-api-audience");
            
            identity.SetDestinations(_ => [Destinations.AccessToken, "refresh_token"]);

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        
        return BadRequest(new OpenIddictResponse
        {
            Error = Errors.InvalidGrant,
            ErrorDescription = "The specified grant type is not supported."
        });
    }
    
    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not null)
        {
            return BadRequest("Username already exists.");
        }
 
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = request.Username,
            Email = request.Email
        };
 
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
 
        return Ok("User registered successfully.");
    }
}