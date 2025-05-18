using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using ReportHub.Identity.Application.Features.Auth.Commands;
using ReportHub.Identity.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
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

        var newPrinciple = await _mediator.Send(new RefreshTokenCommand(principal));

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [HttpPost("/auth/select-client")]
    public async Task<IActionResult> ClientToken()
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
        var clientId = request.GetParameter("userClientId")!.ToString();

        var newPrinciple = await _mediator.Send(new ClientTokenCommand(clientId!));

        return SignIn(newPrinciple, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private OpenIddictRequest GetOpenIddictRequest()
    {
        return HttpContext.GetOpenIddictServerRequest()
                      ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
    }
}