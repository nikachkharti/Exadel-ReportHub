using Microsoft.AspNetCore.Authorization;
using ReportHub.API.Authorization.Permissions;

namespace ReportHub.API.Authorization.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class PermissionAttribute : AuthorizeAttribute
{
    public PermissionType Permission { get; }
    public PermissionAttribute(PermissionType permission) 
    {
        Permission = permission;
        Policy = permission.ToString();
    }
}

