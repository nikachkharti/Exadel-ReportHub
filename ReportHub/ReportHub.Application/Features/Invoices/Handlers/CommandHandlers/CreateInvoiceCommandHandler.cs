using MediatR;
using ReportHub.Application.Contracts.CurrencyContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Invoices.Commands;
using ReportHub.Application.Validators.Exceptions;
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
        var items = new List<Item>();
        foreach(var itemId in request.items)
        {
            var item = await _itemRepository.Get(i =>  i.Id == itemId);
            if(item == null)
            {
                throw new NotFoundException($"Item with Id {itemId} not found.");
            }

            if(item.ClientId !=  request.ClientId)
            {
                throw new BadRequestException($"Client {request.ClientId} does not have this item with Id {itemId}.");
            }
        }


        var customer = await _customerRepository.Get(c => c.Id == request.CustomerId);
        var customerCurrency = await _currencyRepository.Get(c => c.CountryId == customer.CountryId);
        var totalAmount = 0m;
        foreach (var item in items)
        {
            decimal rate = 1;
            if (item.Currency != customerCurrency.Code)
                rate = await _exchangeCurrencyService.GetCurrencyAsync(item.Currency, customerCurrency.Code);
            

                rate = await _exchangeCurrencyService.GetCurrencyAsync(item.Currency, customerCurrency.Code);
            totalAmount += rate * item.Price;

        }

        var invoice = new Invoice
        {
            Amount = totalAmount,
            ClientId = request.ClientId,
            CustomerId = request.CustomerId,
            ItemIds = request.items.ToList(),
            Currency = customerCurrency.Code,
            PaymentStatus = "InProgress",
            IssueDate = DateTime.UtcNow.AddDays(-10),
            DueDate = DateTime.UtcNow.AddDays(20)
        };

        await _invoiceRepository.Insert(invoice);
        return invoice;
    }
}
