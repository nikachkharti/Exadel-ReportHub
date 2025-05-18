using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Invoices.DTOs;
using ReportHub.Application.Features.Invoices.Queries;
using ReportHub.Application.Validators.Exceptions;
using Serilog;

namespace ReportHub.Application.Features.Invoices.Handlers.QueryHandlers;

public class GetInvoicesByIdQueryHandler : BaseFeature, IRequestHandler<GetInvoicesByIdQuery, InvoiceForGettingDto>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;

    public GetInvoicesByIdQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper, IRequestContextService requestContext)
        : base(requestContext)
    {
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
    }
    public async Task<InvoiceForGettingDto> Handle(GetInvoicesByIdQuery request, CancellationToken cancellationToken)
    {
        Log.Information($"Fetching Invoice by Id -> {request.Id}", request.Id);

        var invoce = await _invoiceRepository.Get(i => i.Id == request.Id);

        if (invoce is null)
        {
            throw new NotFoundException($"There is no ivoice with id {request.Id}");
        }

        //EnsureUserHasRoleForThisClient(invoce.ClientId);

        var invoiceDto = _mapper.Map<InvoiceForGettingDto>(invoce);

        return invoiceDto;
    }
}
