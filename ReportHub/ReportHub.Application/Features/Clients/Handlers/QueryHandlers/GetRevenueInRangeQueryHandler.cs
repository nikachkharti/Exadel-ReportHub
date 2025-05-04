using MediatR;
using ReportHub.Application.Contracts.CurrencyContracts;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Clients.DTOs;
using ReportHub.Application.Features.Clients.Queries;
using ReportHub.Application.Validators.Exceptions;
using System.Runtime.CompilerServices;

namespace ReportHub.Application.Features.Clients.Handlers.QueryHandlers;

public class GetRevenueInRangeQueryHandler : IRequestHandler<GetRevenueInRangeQuery, RevenueDto>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IExchangeCurrencyService _exchangeCurrencyService;
    private readonly IRequestContextService _requestContextService;

    public GetRevenueInRangeQueryHandler(IInvoiceRepository invoiceRepository, 
        IExchangeCurrencyService exchangeCurrencyService, IRequestContextService requestContextService)
    {
        _invoiceRepository = invoiceRepository;
        _exchangeCurrencyService = exchangeCurrencyService;
        _requestContextService = requestContextService;
    }

    public async Task<RevenueDto> Handle(GetRevenueInRangeQuery request, CancellationToken cancellationToken)
    {
        var cliendId = _requestContextService.GetClientId();
        var invoices = await _invoiceRepository.GetAll
            (i => i.ClientId == cliendId && i.PaymentStatus == "Paid" && i.IssueDate <= request.ToDate && i.IssueDate >= request.FromDate);
        if (invoices == null)
            throw new NotFoundException("This currency is not supported by ECB.");

        decimal totalAmount = 0m;

        foreach (var invoice in invoices)
        {
            var rate = await _exchangeCurrencyService.GetCurrencyAsync(invoice.Currency, request.CurrencyCode);
            totalAmount += invoice.Amount * rate;

        }
        return new RevenueDto(request.CurrencyCode, totalAmount);
    }
}
