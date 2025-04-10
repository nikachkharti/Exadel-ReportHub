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
using System.Threading.Tasks;

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

    [HttpPost("/auth/login-as-admin"), Produces("application/json")]
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

        return await LoginAs(request, "Admin");
    }

    [HttpPost("/auth/login-as-client"), Produces("application/json")]
    public async Task<IActionResult> LoginAsClient()
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

        return await LoginAs(request, "Client");
    }

    private async Task<IActionResult> LoginAs(OpenIddictRequest request, string role)
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

        if (!await _userManager.IsInRoleAsync(result.user!, role))
        {
            return Forbid();
        }

        ClaimsIdentity identity = GetIdentityClaims();

        SetClaims(request, role, result.user!, identity);

        identity.SetScopes(request.GetScopes());

        var principal = new ClaimsPrincipal(identity);

        SetDestinations(principal);

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
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

    private void SetClaims(OpenIddictRequest request, string role, User user, ClaimsIdentity identity)
    {
        identity
                    .SetClaim(Claims.Subject, request.ClientId)
                    .SetClaim(Claims.Audience, "report-hub-api-audience")
                    .SetClaim(Claims.Email, user.Email)
                    .SetClaim(Claims.Name, user.UserName)
                    .SetClaims(Claims.Role, [role]);
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