using ReportHub.Application.Contracts;
using ReportHub.Domain.Entities;

namespace ReportHub.Infrastructure.Repository
{
    public class InvoiceRepository : MongoRepositoryBase<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(string connectionString, string databaseName, string collectionName) : base(connectionString, databaseName, collectionName)
        {
        }
    }
}
