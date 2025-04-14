using MediatR;

namespace ReportHub.Application.Features.Clients.Commands
{
    public record DeleteClientCommand(string Id) : IRequest<string>;
}
