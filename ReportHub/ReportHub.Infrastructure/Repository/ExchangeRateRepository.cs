using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;
using System.Collections.ObjectModel;

namespace ReportHub.Infrastructure.Repository;

public class ExchangeRateRepository : MongoRepositoryBase<ExchangeRate>, IExchangeRateRepository
{
    public ExchangeRateRepository(IOptions<MongoDbSettings> options) : base(options, "Currencies")
    {
    }
}
