using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Items.Commands;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Items.Handlers.CommandHandlers
{
    public class CreateItemCommandHandler(IItemRepository itemRepository, IMapper mapper, IRequestContextService requestContext)
        : IRequestHandler<CreateItemCommand, string>
    {
        public async Task<string> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.
            //EnsureUserHasRoleForThisClient(request.ClientId);

            var item = mapper.Map<Item>(request);

            item.ClientId = clientId;

            await itemRepository.Insert(item);

            return item.Id;
        }
    }
}
