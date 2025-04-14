using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Item.Commands;

namespace ReportHub.Application.Features.Item.Handlers.CommandHandlers
{
    public class CreateItemCommandHandler(IItemRepository itemRepository, IMapper mapper)
        : IRequestHandler<CreateItemCommand, string>
    {
        public async Task<string> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.

            var item = mapper.Map<Domain.Entities.Item>(request);

            await itemRepository.Insert(item);
            return item.Id;
        }
    }
}
