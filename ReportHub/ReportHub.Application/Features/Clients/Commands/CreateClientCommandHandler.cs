using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Clients.Commands;

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Guid>
{
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public CreateClientCommandHandler(IClientRepository clientRepository, IMapper mapper)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var client = _mapper.Map<Client>(request);
        await _clientRepository.Insert(client, cancellationToken);

        return client.Id;
    }
}