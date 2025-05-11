using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Items.Commands;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Items.Handlers.CommandHandlers;

public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, string>
{
    private readonly IRequestContextService _requestContext;
    private readonly IItemRepository _itemRepository;

    public DeleteItemCommandHandler(IRequestContextService requestContext, IItemRepository itemRepository)
    {
        _requestContext = requestContext;
        _itemRepository = itemRepository;
    }

    public async Task<string> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var clientId = _requestContext.GetClientId();

        var existingItem = await _itemRepository.Get(i => i.ClientId == clientId && i.Id == request.Id);

        if(existingItem is null || existingItem.IsDeleted)
        {
            throw new NotFoundException($"Item with id {request.Id} is not found");
        }

        await _itemRepository.Delete(i => i.Id == request.Id && i.ClientId == clientId);

        return "Item deleted successfully";
    }
}
