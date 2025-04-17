using MediatR;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.ClientRoles.Queries;

public record GetClientRoleByNameQuery(string RoleName) : IRequest<ClientRole>;