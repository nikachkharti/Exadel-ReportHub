using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts;
using ReportHub.Application.Features.Clients.Queries;
using ReportHub.Application.Features.Invoices.DTOs;
using Serilog;

namespace ReportHub.Application.Features.Clients.Handlers.QueryHandlers;

public class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, IEnumerable<ClientDto>>
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public GetAllClientsQueryHandler(IClientRepository clientRepository, IMapper mapper)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
    }
    public async Task<IEnumerable<ClientDto>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
    {
        Log.Information("Fetching all Clients");

        var clients = await _clientRepository.GetAll();
        var clientDtos = _mapper.Map<IEnumerable<ClientDto>>(clients);

        Log.Information("Successfully fetched {Count} clients", clientDtos.Count());

        return clientDtos;
    }
}
