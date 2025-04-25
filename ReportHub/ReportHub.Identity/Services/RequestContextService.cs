using ReportHub.Identity.Interfaces.ServiceInterfaces;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ReportHub.Identity.Services;

public class RequestContextService : IRequestContextService
{
    private readonly ClaimsPrincipal _principal;

    public RequestContextService(IHttpContextAccessor httpContextAccessor)
    {
        _principal = httpContextAccessor.HttpContext!.User;

        if(_principal is null)
        {
            throw new ArgumentNullException();
        }
    }

    public string GetUserId()
    {
        return GetClaimValue(ClaimTypes.NameIdentifier);
    }

    public string GetClientId()
    {
        return GetClaimValue("Client");
    }

    public string GetRole()
    {
        return GetClaimValue(Claims.Role);
    }

    private string GetClaimValue(string name)
    {
        var value = _principal.FindFirst(ClaimTypes.NameIdentifier)!.Value ?? throw new ArgumentNullException();

        return value;
    }
}
