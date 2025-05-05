using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Serilog;

namespace ReportHub.Infrastructure.Workers;

public partial class DataSeeder(IServiceProvider serviceProvider) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();

            await AddEcbSupport(scope, context.CancellationToken);
            await SeedClientsAsync(scope, context.CancellationToken);
            await SeedCustomersAsync(scope, context.CancellationToken);
            await SeedItemsAsync(scope, context.CancellationToken);
            await SeedInvoiesAsync(scope, context.CancellationToken);
            await SeedPlansAsync(scope, context.CancellationToken);
            await SeedSalesAsync(scope, context.CancellationToken);
            await SeedReportSchedulesAsync(scope, context.CancellationToken);
            await SeedCountriesAndCurrenciesAsync(scope, context.CancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Data seeding job failed: {ex.Message}");
        }
    }
}
