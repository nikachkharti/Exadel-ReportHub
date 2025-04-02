using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using ReportHub.Identity.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ReportHub.Identity.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly UserManager<User> _userManager;

    public AuthController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    [HttpPost("/connect/token"), Produces("application/json")]
    public async Task<IActionResult> Exchange(SignInRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { error = "Invalid credentials." });
        }
        
        var claims = new List<Claim>
        {
            new(Claims.Subject, user.Id),
            new(Claims.Email, user.Email)
        };

        var identity = new ClaimsIdentity(claims, TokenValidationParameters.DefaultAuthenticationType);
        
        var principal = new ClaimsPrincipal(identity);
        principal.SetScopes(Scopes.OfflineAccess, Scopes.Profile, Scopes.Email);
        
        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    
    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
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