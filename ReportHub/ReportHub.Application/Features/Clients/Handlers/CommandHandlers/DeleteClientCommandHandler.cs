using MediatR;
using ReportHub.Application.Contracts;
using ReportHub.Application.Features.Clients.Commands;
using Serilog;

namespace ReportHub.Application.Features.Clients.Handlers.CommandHandlers
{
    public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, bool>
    {
        private readonly IClientRepository _clientRepository;

        public DeleteClientCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<bool> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            var existingClient = await _clientRepository.Get(x => x.Id == request.ClientId, cancellationToken);
            if (existingClient == null)
            {
                Log.Warning("Client with Id {ClientId} not found", request.ClientId);
                return false;
            }

            await _clientRepository.Delete(x => x.Id == request.ClientId, cancellationToken);
            Log.Information("Client with Id {ClientId} deleted successfully", request.ClientId);
            return true;
        }
    }
}