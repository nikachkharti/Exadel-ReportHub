using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;
using System.Collections.ObjectModel;

namespace ReportHub.Infrastructure.Repository;

public class CurrencyRepository : MongoRepositoryBase<Currency>, ICurrencyRepository
{
    public CurrencyRepository(IOptions<MongoDbSettings> options) : base(options, "Currencies")
    {
    }
}
