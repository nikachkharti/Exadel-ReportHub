using Microsoft.Extensions.Options;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;

namespace ReportHub.Infrastructure.Repository;

public class PermissionRepository : MongoRepositoryBase<Permission>, IPermissionRepository
{
    public PermissionRepository(IOptions<MongoDbSettings> options) : base(options, "Permissions")
    {
    }
}
