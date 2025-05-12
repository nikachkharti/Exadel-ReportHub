using AutoMapper;
using MediatR;
using ReportHub.Application.Common.Helper;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Clients.Queries;
using ReportHub.Application.Features.Customers.Queries;
using ReportHub.Application.Features.Items.DTOs;
using ReportHub.Domain.Entities;
using System.Linq.Expressions;

namespace ReportHub.Application.Features.Clients.Handlers.QueryHandlers
{
    public class GetAllItemsOfClientQueryHandler(IItemRepository itemRepository, IMapper mapper)
        : IRequestHandler<GetAllItemsOfClientQuery, IEnumerable<ItemForGettingDto>>
    {
        public async Task<IEnumerable<ItemForGettingDto>> Handle(GetAllItemsOfClientQuery request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.

            var sortExpression = ConfigureSortingExpression(request);

            var items = await itemRepository
                .GetAll
                (
                    i => i.ClientId == request.ClientId,
                    request.PageNumber ?? 1,
                    request.PageSize ?? 10,
                    sortBy: sortExpression,
                    ascending: request.Ascending,
                    cancellationToken
                );

            if (items.Any())
            {
                return mapper.Map<IEnumerable<ItemForGettingDto>>(items);
            }

            return Enumerable.Empty<ItemForGettingDto>();
        }

        private static Expression<Func<Item, object>> ConfigureSortingExpression(GetAllItemsOfClientQuery request)
        {
            Expression<Func<Item, object>> sortExpression;

            if (!string.IsNullOrWhiteSpace(request.SortingParameter))
            {
                sortExpression = ExpressionBuilder.BuildSortExpression<Item>(request.SortingParameter);
            }
            else
            {
                sortExpression = x => x.Id;
            }

            return sortExpression;
        }

    }
}
