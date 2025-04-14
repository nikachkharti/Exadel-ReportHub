using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Item.DTOs;
using ReportHub.Application.Features.Item.Queries;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Item.Handlers.QueryHandlers
{
    public class GetItemByIdQueryHandler(IItemRepository itemRepository, IMapper mapper)
        : IRequestHandler<GetItemByIdQuery, ItemForGettingDto>
    {
        public async Task<ItemForGettingDto> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
        {
            var item = await itemRepository.Get(i => i.Id == request.Id, cancellationToken);

            if (item is null)
            {
                throw new NotFoundException($"Item with id {request.Id} not found");
            }

            return mapper.Map<ItemForGettingDto>(item);
        }
    }
}
