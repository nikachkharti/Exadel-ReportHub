using Microsoft.Extensions.Options;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;

namespace ReportHub.Infrastructure.Repository
{
    public class SaleRepository : MongoRepositoryBase<Sale>, ISaleRepository
    {
        public SaleRepository(IOptions<MongoDbSettings> options) : base(options, "sales")
        {
        }
    }
}
