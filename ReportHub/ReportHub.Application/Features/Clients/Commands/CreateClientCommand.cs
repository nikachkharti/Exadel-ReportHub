using MediatR;
using ReportHub.Application.Features.Clients.DTOs;

namespace ReportHub.Application.Features.Clients.Commands
{
    public record CreateClientCommand(ClientForCreatingDto Command) : IRequest<string>;
}
