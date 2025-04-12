using MediatR;

namespace ReportHub.Application.Features.Clients.Commands
{
    public class DeleteClientCommand : IRequest<bool>
    {
        public Guid ClientId { get; set; }
    }
}