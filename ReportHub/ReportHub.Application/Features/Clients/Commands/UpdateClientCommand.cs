using MediatR;
using ReportHub.Application.Features.Clients.DTOs;

namespace ReportHub.Application.Features.Clients.Commands
{
    public record UpdateClientCommand(ClientForUpdatingDto Command) : IRequest<string>;

}
