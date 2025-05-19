using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Items.Commands;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Items.Handlers.CommandHandlers
{
    public class DeleteItemOfClientCommandHandler(IItemRepository itemRepository, IRequestContextService requestContext)
        : BaseFeature(requestContext), IRequestHandler<DeleteItemOfClientCommand, string>
    {
        public async Task<string> Handle(DeleteItemOfClientCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.

            //EnsureUserHasRoleForThisClient(request.ClientId); TODO Uncomment when I add authorization on web

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
