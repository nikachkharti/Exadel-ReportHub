using MediatR;
using ReportHub.Application.Contracts.CurrencyContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Invoices.Commands;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Invoices.Handlers.CommandHandlers;

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Invoice>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IExchangeCurrencyService _exchangeCurrencyService;
    private readonly IItemRepository _itemRepository;
    private readonly ICustomerRepository _customerRepository;
    public CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepository, ICurrencyRepository currencyRepository, IExchangeCurrencyService exchangeCurrencyService, IItemRepository itemRepository, ICustomerRepository customerRepository)
    {
        _invoiceRepository = invoiceRepository;
        _currencyRepository = currencyRepository;
        _exchangeCurrencyService = exchangeCurrencyService;
        _itemRepository = itemRepository;
        _customerRepository = customerRepository;
    }
    public async Task<Invoice> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var items = new List<ReportHub.Domain.Entities.Item>();
        foreach(var itemId in request.items)
        {
            items.Add(await _itemRepository.Get(i =>  i.Id == itemId));
        }
        var customer = await _customerRepository.Get(c => c.Id == request.CustomerId);
        var currency = await _currencyRepository.Get(c => c.CountryId == customer.CountryId);
        var totalAmount = 0m;
        foreach(var item in items)
        {
            var itemCurrency = await _exchangeCurrencyService.GetCurrencyAsync(item.Currency, currency.Code);
        }

        return null;
    }
}
