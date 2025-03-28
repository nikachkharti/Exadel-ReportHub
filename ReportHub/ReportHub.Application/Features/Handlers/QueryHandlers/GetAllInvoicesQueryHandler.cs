using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ReportHub.Application.Contracts;
using ReportHub.Application.Features.DTOs;
using ReportHub.Application.Features.Queries;

namespace ReportHub.Application.Features.Handlers.QueryHandlers;

public class GetAllInvoicesQueryHandler : IRequestHandler<GetAllInvoicesQuery, IEnumerable<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<GetAllInvoicesQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetAllInvoicesQueryHandler(IInvoiceRepository invoiceRepository, 
        ILogger<GetAllInvoicesQueryHandler> logger,
        IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public Task<IEnumerable<InvoiceDto>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
    {
        var invoices = _invoiceRepository.GetAll().GetAwaiter().GetResult();

        var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(invoices);

        return Task.FromResult(invoiceDtos);
    }
}
