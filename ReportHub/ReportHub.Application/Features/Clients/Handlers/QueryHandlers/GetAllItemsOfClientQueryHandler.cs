using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Clients.Queries;
using ReportHub.Application.Features.Items.DTOs;

namespace ReportHub.Application.Features.Clients.Handlers.QueryHandlers
{
    public class GetAllItemsOfClientQueryHandler(IItemRepository itemRepository, IMapper mapper)
        : IRequestHandler<GetAllItemsOfClientQuery, IEnumerable<ItemForGettingDto>>
    {
        public async Task<IEnumerable<ItemForGettingDto>> Handle(GetAllItemsOfClientQuery request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.

            var items = await itemRepository
                .GetAll(i => i.ClientId == request.ClientId, cancellationToken);

            if (items.Any())
            {
                return mapper.Map<IEnumerable<ItemForGettingDto>>(items);
            }

            return Enumerable.Empty<ItemForGettingDto>();
        }
    }
}
