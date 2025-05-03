using AutoMapper;
using MediatR;
using MongoDB.Driver;
using ReportHub.Application.Common.Helper;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Invoices.Queries;
using ReportHub.Application.Validators.Exceptions;
using ReportHub.Domain.Entities;
using System.Linq.Expressions;

namespace ReportHub.Application.Features.Invoices.Handlers.QueryHandlers
{
    public class GetAllInvoicesInADateRangeQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        : IRequestHandler<GetAllInvoicesInADateRangeQuery, int>
    {
        public async Task<int> Handle(GetAllInvoicesInADateRangeQuery request, CancellationToken cancellationToken)
        {
            if (request.StartDate > request.EndDate)
                throw new BadRequestException("Start date value can't be greater than end date");

            var sortExpression = ConfigureSortingExpression(request);

            var filterBuilder = Builders<Invoice>.Filter;

            var filters = new List<FilterDefinition<Invoice>>()
            {
                filterBuilder.Gte(x => x.IssueDate, request.StartDate),
                filterBuilder.Lte(x => x.DueDate, request.EndDate)
            };

            if (!string.IsNullOrWhiteSpace(request.ClientId))
                filters.Add(filterBuilder.Eq(x => x.ClientId, request.ClientId));

            if (!string.IsNullOrWhiteSpace(request.CustomerId))
                filters.Add(filterBuilder.Eq(x => x.CustomerId, request.CustomerId));

            var filter = filterBuilder.And(filters);

            var invoices = await invoiceRepository.GetAll
            (
                filter,
                request.PageNumber ?? 1,
                request.PageSize ?? 10,
                sortBy: sortExpression,
                ascending: request.Ascending,
                cancellationToken
            );

            return invoices.Any()
                ? invoices.Count()
                : 0;
        }


        /// <summary>
        /// Sorting expression configuration with reflection if sorting argument is provided
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Expression Inovice Log Object</returns>
        private Expression<Func<Invoice, object>> ConfigureSortingExpression(GetAllInvoicesInADateRangeQuery request)
        {
            Expression<Func<Invoice, object>> sortExpression;

            if (!string.IsNullOrWhiteSpace(request.SortingParameter))
            {
                sortExpression = ExpressionBuilder.BuildSortExpression<Invoice>(request.SortingParameter);
            }
            else
            {
                sortExpression = x => x.Id;
            }

            return sortExpression;
        }
    }
}
