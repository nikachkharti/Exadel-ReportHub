using Microsoft.AspNetCore.Authorization;
using ReportHub.API.Authorization.Requirements;
using ReportHub.Application.Contracts.RepositoryContracts;
using System.Security.Claims;

namespace ReportHub.API.Authorization.Handlers;

public class ClientRoleHandler : AuthorizationHandler<ClientRoleRequirement>
{
    private readonly IClientUserRepository _clientUserRepository;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientRoleHandler(IClientUserRepository clientUserRepository, IHttpContextAccessor httpContextAccessor)
    {
        _clientUserRepository = clientUserRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ClientRoleRequirement requirement)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return;

        var httpContext = _httpContextAccessor.HttpContext;
        var routeValues = httpContext?.Request?.RouteValues;

        if (routeValues == null || !routeValues.TryGetValue("clientId", out var clientIdObj))
            return;

        var clientId = clientIdObj?.ToString();

        if (string.IsNullOrWhiteSpace(clientId))
            return;

        var clientUser = await _clientUserRepository.Get(r => r.UserId == userId && r.ClientId == clientId);

        if (clientUser is null)
            return;

        var hasAccess = requirement.RequiredRoles.Any(r => r.Equals(clientUser?.Role));


        if (hasAccess)
            context.Succeed(requirement);
    }
}
