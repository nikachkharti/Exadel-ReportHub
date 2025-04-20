using MediatR;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.ClientRoles.Commands;

public record CreateClientRoleCommand(string RoleName) : IRequest<ClientRole>;