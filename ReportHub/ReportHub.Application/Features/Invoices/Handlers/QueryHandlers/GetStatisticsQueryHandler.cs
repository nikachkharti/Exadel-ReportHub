using MediatR;
using ReportHub.Application.Contracts.CurrencyContracts;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Invoices.DTOs;
using ReportHub.Application.Features.Invoices.Queries;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Invoices.Handlers.QueryHandlers;

public class GetStatisticsQueryHandler : IRequestHandler<GetStatistcsQuery, StatisticsDto>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IExchangeCurrencyService _exchangeCurrencyService;
    private readonly IRequestContextService _requestContextService;
    private readonly ICurrencyRepository _currencyRepository;

    public GetStatisticsQueryHandler(IExchangeCurrencyService exchangeCurrencyService,
        IInvoiceRepository invoiceRepository,
        IRequestContextService requestContextService,
        ICurrencyRepository currencyRepository)
    {
        _exchangeCurrencyService = exchangeCurrencyService;
        _invoiceRepository = invoiceRepository;
        _requestContextService = requestContextService;
        _currencyRepository = currencyRepository;
    }

    public async Task<StatisticsDto> Handle(GetStatistcsQuery request, CancellationToken cancellationToken)
    {
        var clientId = _requestContextService.GetClientId();

        var invoices = await _invoiceRepository
                                    .GetAll(
                                    i => i.ClientId == clientId &&
                                    i.DueDate < DateTime.UtcNow &&
                                    i.PaymentStatus == request.PaymentStatus);

        var currency = await _currencyRepository.Get(c => c.Code.Equals(request.Currency));

        if(currency is null)
        {
            throw new BadRequestException($"Currency [{request.Currency}] is not supported by the European Central Bank for conversion");
        }

        if(invoices is null || !invoices.Any())
        {
            throw new NotFoundException($"No invoice found with status {request.PaymentStatus}");
        }

        decimal totalAmount = 0;

        foreach(var invoice in invoices)
        {
            var rate = await _exchangeCurrencyService.GetCurrencyAsync(invoice.Currency, request.Currency);

            totalAmount += rate * invoice.Amount;
        }

        return new StatisticsDto(invoices.Count(), totalAmount, request.Currency);
    }
}
