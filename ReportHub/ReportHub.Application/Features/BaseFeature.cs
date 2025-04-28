using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features;

public abstract class BaseFeature(IRequestContextService requestContextService)
{
    protected void EnsureUserHasRoleForThisClient(string clientId)
    {
        var tokenClient = requestContextService.GetClientId();

        if(tokenClient != clientId)
        {
            throw new UnauthorizedException("You do not have access for this resource");
        }
    }
}
