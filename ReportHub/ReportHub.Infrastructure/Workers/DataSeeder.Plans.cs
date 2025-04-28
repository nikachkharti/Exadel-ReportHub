using Microsoft.Extensions.DependencyInjection;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using Serilog;

namespace ReportHub.Infrastructure.Workers;

public partial class DataSeeder
{
    private async Task SeedPlansAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var planRepository = scope.ServiceProvider.GetRequiredService<IPlanRepository>();

        var existingPlans = await planRepository.GetAll(pageNumber: 1, pageSize: 1);

        if (existingPlans.Any())
        {
            Log.Information("Database already contains plan data. Skipping seeding...");
            return;
        }

        Log.Information("Seeding initial plans data...");

        var plans = GetPlans();

        await planRepository.InsertMultiple(plans, cancellationToken);

        Log.Information("Plan seeding completed");
    }

    private IEnumerable<Plan> GetPlans()
    {
        return new List<Plan>()
        {
            new Plan()
            {
                Id = "680234508ed022f95e0789d9",
                ClientId = "67fa2d8114e2389cd8064452",
                ItemId = "67fa2d8114e2389cd8064460",
                Amount = 3000.00m,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddMonths(3),
                Status = PlanStatus.Planned
            },
            new Plan()
            {
                Id = "680234508ed022f95e0789da",
                ClientId = "67fa2d8114e2389cd8064453",
                ItemId = "67fa2d8114e2389cd8064464",
                Amount = 9000.00m,
                StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddMonths(6),
                Status = PlanStatus.Planned
            },
            new Plan()
            {
                Id = "680234508ed022f95e0789db",
                ClientId = "67fa2d8114e2389cd8064454",
                ItemId = "67fa2d8114e2389cd8064465",
                Amount = 350.00m,
                StartDate = DateTime.UtcNow.AddDays(-5),
                EndDate = DateTime.UtcNow.AddMonths(1),
                Status = PlanStatus.InProgress
            }
        };
    }
}
