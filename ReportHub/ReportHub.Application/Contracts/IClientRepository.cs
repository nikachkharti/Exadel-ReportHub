using ReportHub.Domain.Entities;

namespace ReportHub.Application.Contracts
{
    public interface IClientRepository : IMongoRepositoryBase<Client>
    {
    }
}
