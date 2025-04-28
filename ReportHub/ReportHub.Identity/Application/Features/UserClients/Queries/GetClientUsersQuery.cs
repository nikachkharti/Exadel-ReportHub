using MediatR;
using ReportHub.Identity.Application.Features.UserClients.DTOs;

namespace ReportHub.Identity.Application.Features.UserClients.Queries;

public record GetClientUsersQuery(string clientId) : IRequest<IEnumerable<ClientUserForGettingDto>>;
