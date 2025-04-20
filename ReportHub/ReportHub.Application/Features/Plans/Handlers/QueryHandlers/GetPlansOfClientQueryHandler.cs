using AutoMapper;
using MediatR;
using ReportHub.Application.Common.Helper;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Plans.DTOs;
using ReportHub.Application.Features.Plans.Queries;
using ReportHub.Domain.Entities;
using System.Linq.Expressions;

namespace ReportHub.Application.Features.Plans.Handlers.QueryHandlers
{
    public class GetPlansOfClientQueryHandler(IPlanRepository planRepository, IMapper mapper)
        : IRequestHandler<GetPlansOfClientQuery, IEnumerable<PlanForGettingDto>>
    {
        public async Task<IEnumerable<PlanForGettingDto>> Handle(GetPlansOfClientQuery request, CancellationToken cancellationToken)
        {
            var sortExpression = ConfigureSortingExpression(request);

            var plans = await planRepository.GetAll
                (
                    p => p.ClientId == request.Clientid,
                    request.PageNumber ?? 1,
                    request.PageSize ?? 10,
                    sortBy: sortExpression,
                    ascending: request.Ascending,
                    cancellationToken
                );

            if (plans.Any())
            {
                return mapper.Map<IEnumerable<PlanForGettingDto>>(plans);
            }

            return Enumerable.Empty<PlanForGettingDto>();
        }


        /// <summary>
        /// Sorting expression configuration with reflection if sorting argument is provided
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Expression Func Client Object</returns>
        private static Expression<Func<Plan, object>> ConfigureSortingExpression(GetPlansOfClientQuery request)
        {
            Expression<Func<Plan, object>> sortExpression;

            if (!string.IsNullOrWhiteSpace(request.SortingParameter))
            {
                sortExpression = ExpressionBuilder.BuildSortExpression<Plan>(request.SortingParameter);
            }
            else
            {
                sortExpression = x => x.Id;
            }

            return sortExpression;
        }
    }
}
