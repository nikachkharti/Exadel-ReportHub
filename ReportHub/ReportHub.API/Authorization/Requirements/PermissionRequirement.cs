using Microsoft.AspNetCore.Authorization;
using ReportHub.API.Authorization.Permissions;

namespace ReportHub.API.Authorization.Requirements;

public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionType Permission { get; }

    public PermissionRequirement(PermissionType permission)
    {
        Permission = permission;
    }
}
