using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Clients.Commands;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Clients.Handlers.CommandHandlers
{
    public class DeleteClientCommandHandler(IClientRepository clientRepository)
        : IRequestHandler<DeleteClientCommand, string>
    {
        public async Task<string> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.

            var client = await clientRepository.Get(c => c.Id == request.Id, cancellationToken);

            if (client is null)
            {
                throw new NotFoundException($"Client with id {request.Id} not found");
            }

            //TODO: [Optimization] we have to avoid double calling of database.
            await clientRepository.Delete(c => c.Id == request.Id, cancellationToken);
            return client.Id;
        }
    }
}
