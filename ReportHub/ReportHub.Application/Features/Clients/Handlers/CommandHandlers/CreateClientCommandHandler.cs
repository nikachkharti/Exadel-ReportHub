using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Clients.Commands;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Clients.Handlers.CommandHandlers
{
    public class CreateClientCommandHandler(IClientRepository clientRepository, IMapper mapper)
        : IRequestHandler<CreateClientCommand, string>
    {
        public async Task<string> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.
            var client = mapper.Map<Client>(request);

            await clientRepository.Insert(client);
            return client.Id;
        }
    }
}