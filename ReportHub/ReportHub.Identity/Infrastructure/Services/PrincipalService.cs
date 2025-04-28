using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using ReportHub.Identity.Application.Interfaces.ServiceInterfaces;
using ReportHub.Identity.Domain.Entities;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ReportHub.Identity.Infrastructure.Services;

public class PrincipalService : IPrincipalService
{
    public ClaimsPrincipal GetClaimsPrincipal(User user, string client = default!, string role = default!)
    {
        var identityClaims = GetIdentityClaims();

        SetClaims(user, identityClaims, client, role);
        SetScopes(identityClaims);

        if(client is not null || client == "System") identityClaims.SetDestinations(_ => [Destinations.AccessToken, "refresh_token"]);

        var principal = new ClaimsPrincipal(identityClaims);

        if(client is null) SetDestinations(principal);

        return principal;
    }

    private ClaimsIdentity GetIdentityClaims()
    {
        return new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);
    }

    private void SetClaims(User user, ClaimsIdentity identity, string client = default!, string role = default!)
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

    private void SetScopes(ClaimsIdentity identityClaims)
    {
        identityClaims.SetScopes("report-hub-api-scope", "roles", "offline_access");
    }

    private static void SetDestinations(ClaimsPrincipal principal)
    {
        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(Destinations.AccessToken, Destinations.IdentityToken);
        }
    }
}
