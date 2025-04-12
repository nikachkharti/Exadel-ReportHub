using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts;
using Serilog;

namespace ReportHub.Application.Features.Clients.Queries;

public class GetByIdClientQueryHandler : IRequestHandler<GetByIdClientQuery, ClientDto>
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public GetByIdClientQueryHandler(IClientRepository clientRepository, IMapper mapper)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<ClientDto> Handle(GetByIdClientQuery request, CancellationToken cancellationToken)
    {
        Log.Information($"Fetching Client by Id -> {request.Id}", request.Id);
        var client = await _clientRepository.Get(x => x.Id == request.Id, cancellationToken);
        var clientDto = _mapper.Map<ClientDto>(client);

        return clientDto;
    }
}