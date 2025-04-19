using Microsoft.Extensions.Options;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;

namespace ReportHub.Infrastructure.Repository;

public class CountryRepository : MongoRepositoryBase<Country>, ICountryRepository
{
    public CountryRepository(IOptions<MongoDbSettings> options) : base(options, "Countries")
    {
    }
}
