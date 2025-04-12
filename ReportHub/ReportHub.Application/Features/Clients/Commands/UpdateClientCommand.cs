using MediatR;
namespace ReportHub.Application.Features.Clients.Commands;
public class UpdateClientCommand : IRequest<Guid>
{
    public Guid ClientId { get; set; }
    public string Name { get; set; } = default!;
}