using AutoMapper;
using MediatR;
using ReportHub.Application.Common.Helper;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Clients.DTOs;
using ReportHub.Application.Features.Clients.Queries;
using ReportHub.Domain.Entities;
using System.Linq.Expressions;

namespace ReportHub.Application.Features.Clients.Handlers.QueryHandlers
{
    public class GetAllClientsQueryHandler(IClientRepository clientRepository, IMapper mapper)
        : IRequestHandler<GetAllClientsQuery, IEnumerable<ClientForGettingDto>>
    {
        public async Task<IEnumerable<ClientForGettingDto>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
        {
            var sortExpression = ConfigureSortingExpression(request);

            var clients = await clientRepository.GetAll
            (
                request.PageNumber ?? 1,
                request.PageSize ?? 10,
                sortBy: sortExpression,
                ascending: request.Ascending,
                cancellationToken
            );

            if (clients.Any())
            {
                return mapper.Map<IEnumerable<ClientForGettingDto>>(clients);
            }

            return Enumerable.Empty<ClientForGettingDto>();
        }



        /// <summary>
        /// Sorting expression configuration with reflection if sorting argument is provided
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Expression Func Client Object</returns>
        private static Expression<Func<Client, object>> ConfigureSortingExpression(GetAllClientsQuery request)
        {
            Expression<Func<Client, object>> sortExpression;

            if (!string.IsNullOrWhiteSpace(request.SortingParameter))
            {
                sortExpression = ExpressionBuilder.BuildSortExpression<Client>(request.SortingParameter);
            }
            else
            {
                sortExpression = x => x.Id;
            }

            return sortExpression;
        }
    }
}
