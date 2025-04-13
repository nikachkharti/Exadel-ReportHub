using AutoMapper;
using MediatR;
using ReportHub.Application.Common.Helper;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Customers.DTOs;
using ReportHub.Application.Features.Customers.Queries;
using ReportHub.Domain.Entities;
using System.Linq.Expressions;

namespace ReportHub.Application.Features.Customers.Handlers.QueryHandlers
{
    public class GetAllCustomersQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerForGettingDto>>
    {
        public async Task<IEnumerable<CustomerForGettingDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            var sortExpression = ConfigureSortingExpression(request);

            var customers = await customerRepository.GetAll
            (
                request.PageNumber ?? 1,
                request.PageSize ?? 10,
                sortBy: sortExpression,
                ascending: request.Ascending,
                cancellationToken
            );

            if (customers.Any())
            {
                return mapper.Map<IEnumerable<CustomerForGettingDto>>(customers);
            }

            return Enumerable.Empty<CustomerForGettingDto>();
        }

        private static Expression<Func<Customer, object>> ConfigureSortingExpression(GetAllCustomersQuery request)
        {
            Expression<Func<Customer, object>> sortExpression;

            if (!string.IsNullOrWhiteSpace(request.SortingParameter))
            {
                sortExpression = ExpressionBuilder.BuildSortExpression<Customer>(request.SortingParameter);
            }
            else
            {
                sortExpression = x => x.Id;
            }

            return sortExpression;
        }
    }
}
