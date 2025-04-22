using MediatR;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.ClientRoles.Queries;

public record GetAllClientRolesQuery : IRequest<IEnumerable<ClientRole>>;
