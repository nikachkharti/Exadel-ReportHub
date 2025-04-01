using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts;
using ReportHub.Application.Features.Invoices.DTOs;
using Serilog;

namespace ReportHub.Application.Features.Invoices.Queries;

public class GetInvoicesByIdQueryHandler : IRequestHandler<GetInvoicesByIdQuery, InvoiceDto>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;

    public GetInvoicesByIdQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
    }
    public async Task<InvoiceDto> Handle(GetInvoicesByIdQuery request, CancellationToken cancellationToken)
    {
        Log.Information($"Fetching Invoice by Id -> {request.Id}", request.Id);

        var Invoce = await _invoiceRepository.Get(i => i.InvoiceId == request.Id);

        var invoiceDto = _mapper.Map<InvoiceDto>(Invoce);

        return invoiceDto;
    }
}
