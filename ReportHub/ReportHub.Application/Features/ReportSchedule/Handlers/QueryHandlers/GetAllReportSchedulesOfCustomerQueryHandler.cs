using AutoMapper;
using MediatR;
using ReportHub.Application.Common.Helper;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.ReportSchedule.DTOs;
using ReportHub.Application.Features.ReportSchedule.Queries;
using ReportHub.Application.Validators.Exceptions;
using System.Linq.Expressions;

namespace ReportHub.Application.Features.ReportSchedule.Handlers.QueryHandlers
{
    public class GetAllReportSchedulesOfCustomerQueryHandler
        (IReportScheduleRepository reportScheduleRepository, ICustomerRepository customerRepository, IMapper mapper)
        : IRequestHandler<GetAllReportSchedulesOfCustomerQuery, IEnumerable<ReportScheduleForGettingDto>>
    {
        public async Task<IEnumerable<ReportScheduleForGettingDto>> Handle(GetAllReportSchedulesOfCustomerQuery request, CancellationToken cancellationToken)
        {
            await ValidateReportCustomerExists(request, cancellationToken);
            var sortExpression = ConfigureSortingExpression(request);

            var reportSchedules = await reportScheduleRepository.GetAll
            (
                r => r.CustomerId == request.CustomerId,
                request.PageNumber ?? 1,
                request.PageSize ?? 10,
                sortBy: sortExpression,
                ascending: request.Ascending,
                cancellationToken
            );

            if (reportSchedules.Any())
            {
                return mapper.Map<IEnumerable<ReportScheduleForGettingDto>>(reportSchedules);
            }

            return Enumerable.Empty<ReportScheduleForGettingDto>();
        }


        /// <summary>
        /// Sorting expression configuration with reflection if sorting argument is provided
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Expression Inovice Log Object</returns>
        private Expression<Func<Domain.Entities.ReportSchedule, object>> ConfigureSortingExpression(GetAllReportSchedulesOfCustomerQuery request)
        {
            Expression<Func<Domain.Entities.ReportSchedule, object>> sortExpression;

            if (!string.IsNullOrWhiteSpace(request.SortingParameter))
            {
                sortExpression = ExpressionBuilder.BuildSortExpression<Domain.Entities.ReportSchedule>(request.SortingParameter);
            }
            else
            {
                sortExpression = x => x.Id;
            }

            return sortExpression;
        }
        private async Task ValidateReportCustomerExists(GetAllReportSchedulesOfCustomerQuery request, CancellationToken cancellationToken)
        {
            var customer = await customerRepository.Get(c => c.Id == request.CustomerId, cancellationToken);
            if (customer == null)
                throw new NotFoundException($"Customer with id: {request.CustomerId} not found");
        }

    }
}
