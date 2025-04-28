using AutoMapper;
using MediatR;
using ReportHub.Application.Common.Helper;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.InvoiceLogs.DTOs;
using ReportHub.Application.Features.InvoiceLogs.Queries;
using ReportHub.Domain.Entities;
using System.Linq.Expressions;

namespace ReportHub.Application.Features.InvoiceLogs.Handlers.QueryHandlers
{
    public class GetAllInvoiceLogsOfUserQueryHandler(IInvoiceLogRepository invoiceLogRepository, IMapper mapper)
        : IRequestHandler<GetAllInvoiceLogsOfUserQuery, IEnumerable<InvoiceLogForGettingDto>>
    {
        public async Task<IEnumerable<InvoiceLogForGettingDto>> Handle(GetAllInvoiceLogsOfUserQuery request, CancellationToken cancellationToken)
        {
            var sortExpression = ConfigureSortingExpression(request);

            var invoiceLogs = await invoiceLogRepository.GetAll
            (
                i => i.UserId == request.UserId,
                request.PageNumber ?? 1,
                request.PageSize ?? 10,
                sortBy: sortExpression,
                ascending: request.Ascending,
                cancellationToken
            );

            if (invoiceLogs.Any())
            {
                return mapper.Map<IEnumerable<InvoiceLogForGettingDto>>(invoiceLogs);
            }

            return Enumerable.Empty<InvoiceLogForGettingDto>();
        }


        /// <summary>
        /// Sorting expression configuration with reflection if sorting argument is provided
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Expression Inovice Log Object</returns>
        private Expression<Func<InvoiceLog, object>> ConfigureSortingExpression(GetAllInvoiceLogsOfUserQuery request)
        {
            Expression<Func<InvoiceLog, object>> sortExpression;

            if (!string.IsNullOrWhiteSpace(request.SortingParameter))
            {
                sortExpression = ExpressionBuilder.BuildSortExpression<InvoiceLog>(request.SortingParameter);
            }
            else
            {
                sortExpression = x => x.Id;
            }

            return sortExpression;
        }
    }
}
