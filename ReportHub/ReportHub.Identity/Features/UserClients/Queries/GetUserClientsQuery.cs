using MediatR;
using ReportHub.Identity.Features.UserClients.DTOs;

namespace ReportHub.Identity.Features.UserClients.Queries;

public record GetUserClientsQuery(string UserId) : IRequest<IEnumerable<UserClientForGettingDto>>;