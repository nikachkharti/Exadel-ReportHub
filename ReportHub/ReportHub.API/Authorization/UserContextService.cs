using ReportHub.Application.Contracts.IdentityContracts;
using System.Security.Claims;

namespace ReportHub.API.Authorization
{
    public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
    {
        public string GetUserId() => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
