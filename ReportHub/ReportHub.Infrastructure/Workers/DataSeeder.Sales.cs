using Microsoft.Extensions.DependencyInjection;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using Serilog;

namespace ReportHub.Infrastructure.Workers;

public partial class DataSeeder
{
    private async Task SeedSalesAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var salesRepository = scope.ServiceProvider.GetRequiredService<ISaleRepository>();

        var existingSales = await salesRepository.GetAll(pageNumber: 1, pageSize: 1);

        if (existingSales.Any())
        {
            Log.Information("Database already contains sales data. Skipping seeding...");
            return;
        }

        Log.Information("Seeding initial sales data...");

        var sales = GetSales();

        await salesRepository.InsertMultiple(sales, cancellationToken);

        Log.Information("Sales seeding completed");
    }
    private IEnumerable<Sale> GetSales()
    {
        return new List<Sale>()
        {
            new Sale()
            {
                Id = "67fa2d8114e2389cd8064480",
                ClientId = "67fa2d8114e2389cd8064452",
                ItemId = "67fa2d8114e2389cd8064460", // CRM system building
                Amount = 3000.00m,
                SaleDate = DateTime.UtcNow.AddDays(-15)
            },
            new Sale()
            {
                Id = "67fa2d8114e2389cd8064481",
                ClientId = "67fa2d8114e2389cd8064452",
                ItemId = "67fa2d8114e2389cd8064461", // Landing page
                Amount = 1000.00m,
                SaleDate = DateTime.UtcNow.AddDays(-14)
            },
            new Sale()
            {
                Id = "67fa2d8114e2389cd8064482",
                ClientId = "67fa2d8114e2389cd8064453",
                ItemId = "67fa2d8114e2389cd8064463", // Villa Building
                Amount = 180000.00m,
                SaleDate = DateTime.UtcNow.AddDays(-12)
            },
            new Sale()
            {
                Id = "67fa2d8114e2389cd8064483",
                ClientId = "67fa2d8114e2389cd8064453",
                ItemId = "67fa2d8114e2389cd8064464", // Destroy service
                Amount = 9000.00m,
                SaleDate = DateTime.UtcNow.AddDays(-11)
            },
            new Sale()
            {
                Id = "67fa2d8114e2389cd8064484",
                ClientId = "67fa2d8114e2389cd8064454",
                ItemId = "67fa2d8114e2389cd8064465", // Frontend course
                Amount = 350.00m,
                SaleDate = DateTime.UtcNow.AddDays(-9)
            },
            new Sale()
            {
                Id = "67fa2d8114e2389cd8064485",
                ClientId = "67fa2d8114e2389cd8064454",
                ItemId = "67fa2d8114e2389cd8064466", // Fullstack course
                Amount = 700.00m,
                SaleDate = DateTime.UtcNow.AddDays(-8)
            }
        };
    }
}
