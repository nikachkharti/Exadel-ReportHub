using Microsoft.Extensions.Options;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Infrastructure.Configurations;
using ReportHub.Domain.Entities;

namespace ReportHub.Infrastructure.Repository
{
    public class ItemRepository : MongoRepositoryBase<Item>, IItemRepository
    {
        public ItemRepository(IOptions<MongoDbSettings> options) : base(options, "items")
        {
        }
    }
}
