using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.InvoiceLogs.Commands;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.InvoiceLogs.Handlers.CommandHandlers
{
    public class CreateInvoiceLogCommandHandler(IInvoiceLogRepository invoiceLogRepository, IMapper mapper)
        : IRequestHandler<CreateInvoiceLogCommand, string>
    {
        public async Task<string> Handle(CreateInvoiceLogCommand request, CancellationToken cancellationToken)
        {
            var invoiceLog = mapper.Map<InvoiceLog>(request);

            await invoiceLogRepository.Insert(invoiceLog, cancellationToken);
            return invoiceLog.Id;
        }
    }
}
