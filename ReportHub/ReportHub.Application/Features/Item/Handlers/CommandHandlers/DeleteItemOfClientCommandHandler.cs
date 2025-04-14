using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Item.Commands;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Item.Handlers.CommandHandlers
{
    public class DeleteItemOfClientCommandHandler(IItemRepository itemRepository)
        : IRequestHandler<DeleteItemOfClientCommand, string>
    {
        public async Task<string> Handle(DeleteItemOfClientCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.
            var item = await itemRepository.Get(i => i.Id == request.ItemId, cancellationToken);

            if (item is null)
            {
                throw new NotFoundException($"Item with id {request.ItemId} not found");
            }

            if (item.ClientId != request.ClientId)
            {
                throw new BadRequestException($"Item {request.ItemId} is not an item of client {request.ClientId}");
            }

            //TODO: [Optimization] we have to avoid double calling of database.
            await itemRepository.Delete(c => c.Id == request.ItemId, cancellationToken);
            return item.Id;
        }
    }
}
