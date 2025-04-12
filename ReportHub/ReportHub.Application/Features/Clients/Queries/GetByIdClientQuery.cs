using MediatR;
using ReportHub.Application.Contracts;

namespace ReportHub.Application.Features.Clients.Queries;

public class GetByIdClientQuery : IRequest<ClientDto>
{
    public Guid Id { get; set; }
}
