using Microsoft.AspNetCore.Authorization;

namespace ReportHub.API.Authorization.Requirements;

public class ClientRoleRequirement : IAuthorizationRequirement
{
    public IList<string> RequiredRoles { get; }

    public ClientRoleRequirement(IList<string> requiredRoles)
    {
        RequiredRoles = requiredRoles;
    }
}
