using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using Serilog;

namespace ReportHub.Application.Workers
{
    /// <summary>
    /// Background service to check if plan is expired, executed every hour.
    /// </summary>
    /// <param name="serviceScopeFactory">Injected services</param>
    public class PlanExpireWorker(IServiceScopeFactory serviceScopeFactory) : BackgroundService
    {
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Plan expiration check background job is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var planRepository = scope.ServiceProvider.GetRequiredService<IPlanRepository>();

                    var now = DateTime.UtcNow;

                    var expiredPlans = await planRepository.GetAll
                    (
                        p => p.Status == Domain.Entities.PlanStatus.InProgress &&
                        p.EndDate <= DateTime.UtcNow,
                        stoppingToken
                    );

                    if (expiredPlans.Any())
                    {
                        foreach (var plan in expiredPlans)
                        {
                            await planRepository.UpdateSingleField
                            (
                                p => p.Status == Domain.Entities.PlanStatus.InProgress &&
                                p.EndDate <= now,
                                p => p.Status,
                                PlanStatus.Canceled,
                                stoppingToken
                            );

                            Log.Information($"Plan {plan.Id} expired and marked as canceled.");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error occured in plan expiration background job");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            Log.Information("Plan expiration background job is stopping.");
        }
    }
}
