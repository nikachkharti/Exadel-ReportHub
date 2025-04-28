using Microsoft.IdentityModel.Tokens;
using ReportHub.Identity.Application.Validators.Exceptions;
using ReportHub.Identity.Domain.Entities;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Security.Claims;
using OpenIddict.Abstractions;

namespace ReportHub.Identity.Application.Features.Auth.Handlers.CommandHandlers;

public abstract class BaseAuthCommandHandler
{
    protected ClaimsIdentity GetIdentityClaims()
    {
        return new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);
    }

    protected void SetClaims(User user, ClaimsIdentity identity, string client = default!, string role = default!)
    {
        identity
                    .SetClaim(Claims.Subject, "report-hub")
                    .SetClaim(ClaimTypes.NameIdentifier, user.Id)
                    .SetClaim(Claims.Audience, "report-hub-api-audience")
                    .SetClaim(Claims.Email, user.Email)
                    .SetClaim(Claims.Name, user.UserName);

        if (client is not null)
        {
            identity.SetClaim("Client", client);
        }

        if (role is not null)
        {
            identity.SetClaim(Claims.Role, role);
        }
    }

    protected void SetScopes(ClaimsIdentity identityClaim)
    {
        identityClaim.SetScopes("report-hub-api-scope", "roles", "offline_access");
    }

    protected void EnsurePrincipalsNotNull(ClaimsPrincipal principal)
    {
        if (principal is null)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }
    }
}
