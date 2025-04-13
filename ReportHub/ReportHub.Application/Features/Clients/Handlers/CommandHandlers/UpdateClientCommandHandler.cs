using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Clients.Commands;
using ReportHub.Application.Validators.Exceptions;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Clients.Handlers.CommandHandlers
{
    public class UpdateClientCommandHandler(IClientRepository clientRepository, IMapper mapper)
        : IRequestHandler<UpdateClientCommand, string>
    {
        public async Task<string> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.

            var existingClient = await clientRepository.Get(c => c.Id == request.Id, cancellationToken);

            if (existingClient is null)
            {
                throw new NotFoundException($"Client with id {request.Id} not found");
            }

            var updatedDocumentOfClient = mapper.Map<Client>(request);

            //TODO: [Optimization] we have to avoid double calling of database.
            await clientRepository.UpdateSingleDocument(c => c.Id == request.Id, updatedDocumentOfClient);
            return updatedDocumentOfClient.Id;
        }
    }
}
