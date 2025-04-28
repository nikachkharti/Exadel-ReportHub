using MediatR;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using ReportHub.Identity.Application.Features.Auth.Commands;
using ReportHub.Identity.Application.Interfaces.ServiceInterfaces;
using ReportHub.Identity.Application.Validators.Exceptions;
using ReportHub.Identity.Domain.Entities;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ReportHub.Identity.Application.Features.Auth.Handlers.CommandHandlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ClaimsPrincipal>
{
    private readonly UserManager<User> _userManager;
    private readonly IPrincipalService _principalService;

    public LoginCommandHandler(UserManager<User> userManager, IPrincipalService principalService)
    {
        _userManager = userManager;
        _principalService = principalService;
    }

    public async Task<ClaimsPrincipal> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await GetIfUserExist(request.UserName, request.Password);

        var principal = _principalService.GetClaimsPrincipal(user);

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
}
