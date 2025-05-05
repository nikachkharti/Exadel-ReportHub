using Microsoft.Extensions.DependencyInjection;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;

namespace ReportHub.Infrastructure.Workers
{
    public partial class DataSeeder
    {
        private async Task SeedCustomersAsync(IServiceScope scope, CancellationToken cancellationToken)
        {
            var customerRepository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();

            var existingCustomers = await customerRepository.GetAll(pageNumber: 1, pageSize: 1);

            if (existingCustomers.Any()) return;

            var customers = GetCustomers();

            await customerRepository.InsertMultiple(customers, cancellationToken);
        }

        private IEnumerable<Customer> GetCustomers()
        {
            return new List<Customer>()
            {
                new Customer()
                {
                    Id = "67fa2d8114e2389cd8064457",
                    Name = "John Doe",
                    Email = "jonhode1@gmail.com",
                    CountryId = "680398332b1400012193855f",
                    ClientId = "67fa2d8114e2389cd8064454",
                    IsDeleted = false
                },
                new Customer()
                {
                    Id = "67fa2d8114e2389cd8064458",
                    Name = "Bill Butcher",
                    Email = "bb@gmail.com",
                    CountryId = "680398332b14000121938563",
                    ClientId = "67fa2d8114e2389cd8064453",
                    IsDeleted = false
                },
                new Customer()
                {
                    Id = "67fa2d8114e2389cd8064459",
                    Name = "Freddy Krueger",
                    Email = "freddy@gmail.com",
                    CountryId = "680398332b14000121938563",
                    ClientId = "67fa2d8114e2389cd8064454",
                    IsDeleted = false
                },
                new Customer()
                {
                    Id = "67fa2d8114e2389cd806445a",
                    Name = "John Cenna",
                    Email = "joncena@gmail.com",
                    CountryId = "680398332b1400012193855f",
                    ClientId = "67fa2d8114e2389cd8064452",
                    IsDeleted = false
                }
            };
        }
    }
}
