using AutoMapper;
using MediatR;
using MongoDB.Driver;
using ReportHub.Application.Common.Helper;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.InvoiceLogs.DTOs;
using ReportHub.Application.Features.InvoiceLogs.Queries;
using ReportHub.Application.Validators.Exceptions;
using ReportHub.Domain.Entities;
using System.Linq.Expressions;

namespace ReportHub.Application.Features.InvoiceLogs.Handlers.QueryHandlers
{
    public class GetAllInvoiceLogsInDateRangeQueryHandler(IInvoiceLogRepository invoiceLogRepository, IMapper mapper)
        : IRequestHandler<GetAllInvoiceLogsInDateRangeQuery, IEnumerable<InvoiceLogForGettingDto>>
    {
        public async Task<IEnumerable<InvoiceLogForGettingDto>> Handle(GetAllInvoiceLogsInDateRangeQuery request, CancellationToken cancellationToken)
        {
            if (request.From > request.To)
                throw new BadRequestException("Start date value can't be greater than end date");

            var sortExpression = ConfigureSortingExpression(request);

            var filterBuilder = Builders<InvoiceLog>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Gte(x => x.TimeStamp, request.From),
                filterBuilder.Lte(x => x.TimeStamp, request.To)
            );

            var invoiceLogs = await invoiceLogRepository.GetAll
            (
                filter,
                request.PageNumber ?? 1,
                request.PageSize ?? 50,
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
        private Expression<Func<InvoiceLog, object>> ConfigureSortingExpression(GetAllInvoiceLogsInDateRangeQuery request)
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
