using MediatR;

namespace ReportHub.Application.Features.Clients.Commands;

public class CreateClientCommand : IRequest<Guid>
{
    public string Name { get; set; } = default!;
}
