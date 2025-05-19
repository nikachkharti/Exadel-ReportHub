using AutoMapper;
using MediatR;
using ReportHub.Application.Common.Helper;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Customers.DTOs;
using ReportHub.Application.Features.Customers.Queries;
using ReportHub.Domain.Entities;
using System.Linq.Expressions;

namespace ReportHub.Application.Features.Customers.Handlers.QueryHandlers
{
    public class GetAllCustomersQueryHandler(ICustomerRepository customerRepository, IMapper mapper, IRequestContextService requestContext)
        : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerForGettingDto>>
    {
        public async Task<IEnumerable<CustomerForGettingDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            var sortExpression = ConfigureSortingExpression(request);

            //EnsureUserHasRoleForThisClient(request.ClientId);

            var clientId = requestContext.GetClientId();

            var customers = await customerRepository.GetAll
            (
                c => c.ClientId == clientId,
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
