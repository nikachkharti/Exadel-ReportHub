using MediatR;
using ReportHub.Application.Contracts;

namespace ReportHub.Application.Features.Clients.Queries;

public class GetAllClientsQuery : IRequest<IEnumerable<ClientDto>>
{
}
