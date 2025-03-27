using Microsoft.Extensions.Options;
using ReportHub.Application.Contracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Helper;

namespace ReportHub.Infrastructure.Repository
{
    public class InvoiceRepository : MongoRepositoryBase<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(IOptions<MongoDbSettings> options)
            : base(options, "Invoices") { }
    }
}
