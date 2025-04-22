using Microsoft.Extensions.Options;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;

namespace ReportHub.Infrastructure.Repository;

public class ClientRoleRepository : MongoRepositoryBase<ClientRole>, IClientRoleRepository
{
    public ClientRoleRepository(IOptions<MongoDbSettings> options) : base(options, "ClientRoles")
    {
    }
}
