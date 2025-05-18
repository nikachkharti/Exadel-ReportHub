using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Items.Commands;

namespace ReportHub.Application.Features.Items.Handlers.CommandHandlers
{
    public class CreateItemCommandHandler(IItemRepository itemRepository, IMapper mapper, IRequestContextService requestContext)
        : BaseFeature(requestContext), IRequestHandler<CreateItemCommand, string>
    {
        public async Task<string> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.
            //EnsureUserHasRoleForThisClient(request.ClientId);

            var item = mapper.Map<Domain.Entities.Item>(request);


            await itemRepository.Insert(item);
            return item.Id;
        }
    }
}
