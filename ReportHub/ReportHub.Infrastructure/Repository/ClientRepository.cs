using Microsoft.Extensions.Options;
using ReportHub.Application.Contracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;

namespace ReportHub.Infrastructure.Repository
{
    public class ClientRepository : MongoRepositoryBase<Client>, IClientRepository
    {
        public ClientRepository(IOptions<MongoDbSettings> settings)
            : base(settings, "Clients") { }
    }
}
