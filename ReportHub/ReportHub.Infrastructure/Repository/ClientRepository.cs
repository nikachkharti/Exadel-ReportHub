using Microsoft.Extensions.Options;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;

namespace ReportHub.Infrastructure.Repository
{
    public class ClientRepository : MongoRepositoryBase<Client>, IClientRepository
    {
        public ClientRepository(IOptions<MongoDbSettings> options) : base(options, "clients")
        {
        }
    }
}
