using MongoDB.Bson.Serialization.IdGenerators;
using ReportHub.Identity.Domain.Entities;
using System.Security.Claims;

namespace ReportHub.Identity.Application.Interfaces.ServiceInterfaces;

public interface IPrincipalService
{
    ClaimsPrincipal GetClaimsPrincipal(User user, string client = default!, string role = default!);
}
