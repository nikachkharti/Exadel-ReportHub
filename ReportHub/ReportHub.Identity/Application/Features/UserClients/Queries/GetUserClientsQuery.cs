using MediatR;
using ReportHub.Identity.Application.Features.UserClients.DTOs;

namespace ReportHub.Identity.Application.Features.UserClients.Queries;

public record GetUserClientsQuery(string UserId) : IRequest<IEnumerable<UserClientForGettingDto>>;