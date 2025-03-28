using AutoMapper;
using MediatR;
using Serilog;
using ReportHub.Application.Contracts;
using ReportHub.Application.Features.DTOs;
using ReportHub.Application.Features.Queries;

namespace ReportHub.Application.Features.Handlers.QueryHandlers;

public class GetAllInvoicesQueryHandler : IRequestHandler<GetAllInvoicesQuery, IEnumerable<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public GetAllInvoicesQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper, ILogger logger)
    {
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<InvoiceDto>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
    {
        _logger.Information("Fetching all invoices");

        var invoices = await _invoiceRepository.GetAll();
        var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(invoices);

        _logger.Information("Successfully fetched {Count} invoices", invoiceDtos.Count());

        return invoiceDtos;
    }
}
