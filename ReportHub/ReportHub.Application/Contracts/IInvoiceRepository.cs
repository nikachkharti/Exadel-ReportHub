using ReportHub.Domain.Entities;

namespace ReportHub.Application.Contracts
{
    public interface IInvoiceRepository : IMongoRepositoryBase<Invoice>
    {
    }
}
