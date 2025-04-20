using Microsoft.Extensions.Options;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;

namespace ReportHub.Infrastructure.Repository
{
    public class PlanRepository : MongoRepositoryBase<Plan>, IPlanRepository
    {
        public PlanRepository(IOptions<MongoDbSettings> options) : base(options, "plans")
        {
        }
    }
}
