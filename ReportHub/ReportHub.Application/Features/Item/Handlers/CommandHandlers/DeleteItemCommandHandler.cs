using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Clients.Commands;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Item.Handlers.CommandHandlers
{
    public class DeleteItemCommandHandler(IItemRepository itemRepository)
        : IRequestHandler<DeleteClientCommand, string>
    {
        public async Task<string> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.
            var item = await itemRepository.Get(i => i.Id == request.Id, cancellationToken);

            if (item is null)
            {
                throw new NotFoundException($"Client with id {request.Id} not found");
            }

            //TODO: [Optimization] we have to avoid double calling of database.
            await itemRepository.Delete(c => c.Id == request.Id, cancellationToken);
            return item.Id;
        }
    }
}
