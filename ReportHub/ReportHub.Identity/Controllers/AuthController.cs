using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
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

    public AuthController(
        IOpenIddictApplicationManager applicationManager,
        UserManager<User> userManager)
    {
        _applicationManager = applicationManager;
        _userManager = userManager;
    }
    [Authorize(Roles = "SuperAdmin, Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [HttpPost("users")]
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

        if (request.ClientId is null)
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.InvalidClient,
                ErrorDescription = "The client_id parameter is missing."
            });
        }

        return await Login(request);
    }

    private async Task<IActionResult> Login(OpenIddictRequest request)
    {
        var result = await ValidateUserCreadentials(request.Username!, request.Password!);

        if (!result.isSuccess)
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.InvalidGrant,
                ErrorDescription = "The user name or password is invalid."
            });
        }


        if (await _applicationManager.FindByClientIdAsync(request.ClientId) is null)
            throw new InvalidOperationException("The application cannot be found.");

        var roles = await _userManager.GetRolesAsync(result.user!);

        ClaimsIdentity identity = GetIdentityClaims();

        SetClaims(request, result.user!, identity, (roles is null || roles.Count == 0 ? ["User"] : roles));

        identity.SetScopes(request.GetScopes());

        var principal = new ClaimsPrincipal(identity);

        SetDestinations(principal);

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

        if (request.ClientId is null)
        {
            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.InvalidClient,
                ErrorDescription = "The client_id parameter is missing."
            });
        }

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