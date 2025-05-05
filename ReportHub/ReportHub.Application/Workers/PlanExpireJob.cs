using Quartz;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using Serilog;

public class PlanExpireJob(IPlanRepository planRepository) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            Log.Information("Plan expiration Quartz job is running.");

            var now = DateTime.UtcNow;

            var expiredPlans = await planRepository.GetAll(
                p => p.Status == PlanStatus.InProgress && p.EndDate <= now,
                context.CancellationToken
            );

            if (expiredPlans.Any())
            {
                foreach (var plan in expiredPlans)
                {
                    await planRepository.UpdateSingleField(
                        p => p.Status == PlanStatus.InProgress && p.EndDate <= now,
                        p => p.Status,
                        PlanStatus.Canceled,
                        context.CancellationToken
                    );

                    Log.Information($"Plan {plan.Id} expired and marked as canceled.");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Plan expire job failed: {ex.Message}");
        }
    }
}
