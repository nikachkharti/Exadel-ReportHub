using AutoMapper;
using MediatR;
using Serilog;
using ReportHub.Application.Features.Invoices.DTOs;
using ReportHub.Application.Features.Invoices.Queries;
using ReportHub.Application.Contracts.RepositoryContracts;

namespace ReportHub.Application.Features.Invoices.Handlers.QueryHandlers;

public class GetAllInvoicesQueryHandler : IRequestHandler<GetAllInvoicesQuery, IEnumerable<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;

    public GetAllInvoicesQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<InvoiceDto>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
    {
        Log.Information("Fetching all invoices");

        var invoices = await _invoiceRepository.GetAll();
        var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(invoices);

        Log.Information("Successfully fetched {Count} invoices", invoiceDtos.Count());

        return invoiceDtos;
    }
}
