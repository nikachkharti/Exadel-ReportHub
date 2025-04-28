using MediatR;
using ReportHub.Identity.Application.Features.Roles.DTOs;

namespace ReportHub.Identity.Application.Features.Roles.Queries;

public class GetAllRolesQuery : IRequest<IEnumerable<RoleForGettingDto>>;
