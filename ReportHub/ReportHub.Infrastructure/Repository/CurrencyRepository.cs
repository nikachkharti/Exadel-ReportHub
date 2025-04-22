using Microsoft.Extensions.Options;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;

namespace ReportHub.Infrastructure.Repository;

public class CurrencyRepository : MongoRepositoryBase<Currency>, ICurrencyRepository
{
    public CurrencyRepository(IOptions<MongoDbSettings> options) : base(options, "Currency")
    {
    }
}
