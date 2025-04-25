using MediatR;
using ReportHub.Identity.Features.UserClients.DTOs;

namespace ReportHub.Identity.Features.UserClients.Queries;

public record GetClientUsersQuery(string clientId) : IRequest<IEnumerable<ClientUserForGettingDto>>;
