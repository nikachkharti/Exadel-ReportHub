using Microsoft.Extensions.Options;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;

namespace ReportHub.Infrastructure.Repository
{
    public class CustomerRepository : MongoRepositoryBase<Customer>, ICustomerRepository
    {
        public CustomerRepository(IOptions<MongoDbSettings> options) : base(options, "customers")
        {
        }
    }
}
