using MediatR;

namespace ReportHub.Application.Features.Clients.Commands
{
    public record CreateClientCommand(string Name, string Specialization) : IRequest<string>;
}
