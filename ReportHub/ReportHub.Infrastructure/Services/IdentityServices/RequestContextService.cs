using Microsoft.AspNetCore.Http;
using ReportHub.Application.Contracts.IdentityContracts;
using System.Security.Claims;

namespace ReportHub.Infrastructure.Services.IdentityServices;

public class RequestContextService(IHttpContextAccessor httpContextAccessor) : IRequestContextService
{
    public string GetUserRole()
    {
        return GetClaim(ClaimTypes.Role);
    }

    public string GetClientId()
    {
        return GetClaim("Client");
    }

    private string GetClaim(string name)
    {
        return httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(name)).Value;
    }
}
