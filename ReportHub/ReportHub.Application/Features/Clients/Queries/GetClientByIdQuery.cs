using MediatR;
using ReportHub.Application.Features.Clients.DTOs;

namespace ReportHub.Application.Features.Clients.Queries
{
    public record GetClientByIdQuery(string Id) : IRequest<ClientForGettingDto>;
}
