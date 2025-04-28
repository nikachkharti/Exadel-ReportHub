using Microsoft.Extensions.Options;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;

namespace ReportHub.Infrastructure.Repository
{
    public class InvoiceLogRepository : MongoRepositoryBase<InvoiceLog>, IInvoiceLogRepository
    {
        public InvoiceLogRepository(IOptions<MongoDbSettings> options) : base(options, "invoiceLogs")
        {
        }
    }
}
