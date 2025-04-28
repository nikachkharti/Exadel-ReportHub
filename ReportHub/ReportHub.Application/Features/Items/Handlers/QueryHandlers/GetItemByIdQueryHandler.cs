using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Items.DTOs;
using ReportHub.Application.Features.Items.Queries;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Items.Handlers.QueryHandlers
{
    public class GetItemByIdQueryHandler(IItemRepository itemRepository, IMapper mapper, IRequestContextService requestContext)
        : BaseFeature(requestContext), IRequestHandler<GetItemByIdQuery, ItemForGettingDto>
    {
        public async Task<ItemForGettingDto> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
        {
            var item = await itemRepository.Get(i => i.Id == request.Id, cancellationToken);

            if (item is null)
            {
                throw new NotFoundException($"Item with id {request.Id} not found");
            }

            EnsureUserHasRoleForThisClient(item.ClientId);

            return mapper.Map<ItemForGettingDto>(item);
        }
    }
}
