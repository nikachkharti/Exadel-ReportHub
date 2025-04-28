using MediatR;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using OpenIddict.Abstractions;
using ReportHub.Identity.Application.Features.Auth.Commands;
using ReportHub.Identity.Application.Interfaces.ServiceInterfaces;
using ReportHub.Identity.Application.Validators.Exceptions;
using ReportHub.Identity.Domain.Entities;
using ReportHub.Identity.Infrastructure.Repositories;
using System.Security.Claims;

namespace ReportHub.Identity.Application.Features.Auth.Handlers.CommandHandlers;

public class ClientTokenCommandHandler : IRequestHandler<ClientTokenCommand, ClaimsPrincipal>
{
    private readonly IPrincipalService _principalService;
    private readonly IUserClientRepository _userClientRepository;
    private readonly UserManager<User> _userManager;
    private readonly IRequestContextService _requestContextService;

    public ClientTokenCommandHandler(
        IUserClientRepository userClientRepository,
        IPrincipalService principalService,
        UserManager<User> userManager,
        IRequestContextService requestContextService)
    {
        _userClientRepository = userClientRepository;
        _principalService = principalService;
        _userManager = userManager;
        _requestContextService = requestContextService;
    }

    public async Task<ClaimsPrincipal> Handle(ClientTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = _requestContextService.GetUserId();
        
        var user = await GetUserIfExist(userId);

        var userClient = await GetUserClientIfExist(request.UserClientId);

        var principal = _principalService.GetClaimsPrincipal(
                                            user,
                                            userClient.ClientId ?? "System",
                                            userClient.Role);

        return principal;
    }

    private async Task<UserClient> GetUserClientIfExist(string userClientId)
    {
        if (!ObjectId.TryParse(userClientId, out var clientIdObject))
        {
            throw new BadRequestException("Invalid user client id");
        }

        var userClient = await _userClientRepository
                                .GetAsync(u => u.Id == userClientId);

        if (userClient is null)
        {
            throw new UnauthorizedException("You do not have role for this client");
        }

        return userClient;
    }

    private async Task<User> GetUserIfExist(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId!);

        if (user is null)
        {
            throw new UnauthorizedException("Invalid token");
        }

        return user;
    }
}
