using MediatR;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.CLientUsers.Queries;

public record GetAllClientUserQuery : IRequest<IEnumerable<ClientUser>>;
