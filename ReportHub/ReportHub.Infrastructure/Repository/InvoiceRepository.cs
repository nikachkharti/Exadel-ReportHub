using Microsoft.Extensions.Options;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;

namespace ReportHub.Infrastructure.Repository
{
    public class InvoiceRepository : MongoRepositoryBase<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(IOptions<MongoDbSettings> options) : base(options, "invoices")
        {
        }
    }
}
