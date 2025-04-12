using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts;
using ReportHub.Application.Features.Clients.Commands;
using Serilog;
namespace ReportHub.Application.Features.Clients.Handlers.CommandHandlers;
public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, Guid>
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;
    public UpdateClientCommandHandler(IClientRepository clientRepository, IMapper mapper)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        if (request.ClientId == Guid.Empty) 
        {
            Log.Warning("Client ID was not provided");
            return Guid.Empty;
        }

        Log.Information("Updating client with Id {ClientId}", request.ClientId);
        var existingClient = await _clientRepository.Get(x => x.Id == request.ClientId, cancellationToken);
        if (existingClient == null)
        {
            Log.Warning("Client with Id {ClientId} not found", request.ClientId);
            return Guid.Empty;
        }

        _mapper.Map(request, existingClient);
        await _clientRepository.UpdateSingleDocument(x => x.Id == request.ClientId, existingClient, true, cancellationToken);

        Log.Information("Client with Id {ClientId} updated successfully", request.ClientId);
        return existingClient.Id;
    }
}