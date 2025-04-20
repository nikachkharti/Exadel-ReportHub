using AutoMapper;
using MediatR;
using ReportHub.Application.Common.Helper;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Sale.DTOs;
using ReportHub.Application.Features.Sale.Queries;
using System.Linq.Expressions;

namespace ReportHub.Application.Features.Sale.Handlers.QueryHandlers
{
    public class GetSalesByClientIdQueryHandler(ISaleRepository saleRepository, IMapper mapper)
        : IRequestHandler<GetSalesByClientIdQuery, IEnumerable<SaleForGettingDto>>
    {
        public async Task<IEnumerable<SaleForGettingDto>> Handle(GetSalesByClientIdQuery request, CancellationToken cancellationToken)
        {
            var sortExpression = ConfigureSortingExpression(request);

            var sales = await saleRepository.GetAll
            (
                s => s.ClientId == request.Clientid,
                request.PageNumber ?? 1,
                request.PageSize ?? 10,
                sortBy: sortExpression,
                ascending: request.Ascending,
                cancellationToken
            );

            if (sales.Any())
            {
                return mapper.Map<IEnumerable<SaleForGettingDto>>(sales);
            }

            return Enumerable.Empty<SaleForGettingDto>();
        }

        private Expression<Func<Domain.Entities.Sale, object>> ConfigureSortingExpression(GetSalesByClientIdQuery request)
        {
            Expression<Func<Domain.Entities.Sale, object>> sortExpression;

            if (!string.IsNullOrWhiteSpace(request.SortingParameter))
            {
                sortExpression = ExpressionBuilder.BuildSortExpression<Domain.Entities.Sale>(request.SortingParameter);
            }
            else
            {
                sortExpression = x => x.Id;
            }

            return sortExpression;
        }
    }
}
