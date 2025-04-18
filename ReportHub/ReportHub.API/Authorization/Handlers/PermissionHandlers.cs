using Microsoft.AspNetCore.Authorization;
using ReportHub.API.Authorization.Permissions;
using ReportHub.API.Authorization.Requirements;
using ReportHub.Application.Contracts.RepositoryContracts;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ReportHub.API.Authorization.Handlers;

public class PermissionHandlers : AuthorizationHandler<PermissionRequirement>
{
    private readonly IClientUserRepository _clientUserRepository;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionHandlers(IClientUserRepository clientUserRepository, IHttpContextAccessor httpContextAccessor)
    {
        _clientUserRepository = clientUserRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var roles = context.User.FindAll(Claims.Role)?.Select(r => r.Value);

        if (roles is not null && roles.Any(r => r.Equals("SystemAdmin")))
        {
            context.Succeed(requirement);
            return;
        }

        if (string.IsNullOrEmpty(userId)) return;

        var httpContext = _httpContextAccessor.HttpContext;
        var routeValues = httpContext?.Request?.RouteValues;

        if (routeValues == null || !routeValues.TryGetValue("clientId", out var clientIdObj)) return;

        var clientId = clientIdObj?.ToString();

        if (string.IsNullOrWhiteSpace(clientId)) return;

        var clientRole = await _clientUserRepository.Get(r => r.UserId == userId && r.ClientId == clientId);

        if (clientRole is null) return;

        if (RolePermissions.HasPermission(clientRole.Role, requirement.Permission))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(this, "You do not have permission to work with this data"));
        }
    }
}
