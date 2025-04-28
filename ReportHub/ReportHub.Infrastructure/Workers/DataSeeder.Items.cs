using Microsoft.Extensions.DependencyInjection;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using Serilog;

namespace ReportHub.Infrastructure.Workers;

public partial class DataSeeder
{
    private async Task SeedItemsAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        Log.Information("Seeding initial items data...");

        var itemsRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();

        var existingItems = await itemsRepository.GetAll(pageNumber: 1, pageSize: 1);

        if (existingItems.Any())
        {
            Log.Information("Database already contains items data. Skipping seeding...");
            return;
        }

        var items = GetItems();

        await itemsRepository.InsertMultiple(items);

        Log.Information("Item seeding completed");
    }

    private IEnumerable<Item> GetItems()
    {
        return new List<Item>()
        {
            new Item()
            {
                Id = "67fa2d8114e2389cd8064460",
                ClientId = "67fa2d8114e2389cd8064452",
                Name = "CRM system building",
                Currency = "USD",
                Description = "Building of CRM system for small and medium companies",
                Price = 3000.00m
            },
            new Item()
            {
                Id = "67fa2d8114e2389cd8064461",
                ClientId = "67fa2d8114e2389cd8064452",
                Name = "Landing page building",
                Currency = "USD",
                Description = "Building of landing page",
                Price = 1000.00m
            },
            new Item()
            {
                Id = "67fa2d8114e2389cd8064462",
                ClientId = "67fa2d8114e2389cd8064452",
                Name = "AML monitoring",
                Currency = "USD",
                Description = "Providing an AML monitoring service for small and medium companies",
                Price = 18000.00m
            },
            new Item()
            {
                Id = "67fa2d8114e2389cd8064463",
                ClientId = "67fa2d8114e2389cd8064453",
                Name = "Villa Building",
                Currency = "EUR",
                Description = "Villa building",
                Price = 180000.00m
            },
            new Item()
            {
                Id = "67fa2d8114e2389cd8064464",
                ClientId = "67fa2d8114e2389cd8064453",
                Name = "Destroy service",
                Currency = "EUR",
                Description = "Destroy service for old buildings",
                Price = 9000.00m
            },
            new Item()
            {
                Id = "67fa2d8114e2389cd8064465",
                ClientId = "67fa2d8114e2389cd8064454",
                Name = "Course of frontend software development",
                Currency = "USD",
                Description = "Learn HTML CSS and Javascript",
                Price = 350.00m
            },
            new Item()
            {
                Id = "67fa2d8114e2389cd8064466",
                ClientId = "67fa2d8114e2389cd8064454",
                Name = "Course of fullstrack software development",
                Currency = "USD",
                Description = "Learn full stack programming",
                Price = 700.00m
            }
        };
    }
}
