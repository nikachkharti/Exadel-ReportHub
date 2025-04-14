using MediatR;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.CLientUsers.Queries;

public record GetAllClientUserByClientIdQuery(string ClientId) : IRequest<IEnumerable<ClientUser>>;
