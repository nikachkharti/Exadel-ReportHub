using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Invoices.DTOs;
using ReportHub.Application.Features.Invoices.Queries;
using Serilog;

namespace ReportHub.Application.Features.Invoices.Handlers.QueryHandlers;

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

        var invoce = await _invoiceRepository.Get(i => i.Id == request.Id);

        var invoiceDto = _mapper.Map<InvoiceDto>(invoce);

        return invoiceDto;
    }
}
