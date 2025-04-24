using MediatR;
using ReportHub.Identity.Features.UserClientRoles.DTOs;

namespace ReportHub.Identity.Features.UserClientRoles.Queries;

public record GetMyClientsQuery : IRequest<IEnumerable<MyClientForGettingDto>>;