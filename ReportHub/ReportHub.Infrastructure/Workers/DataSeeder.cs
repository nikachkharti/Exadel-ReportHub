using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ReportHub.Infrastructure.Workers;

public partial class DataSeeder(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var scope = serviceProvider.CreateScope();

            await SeedClientsAsync(scope, cancellationToken);
            await SeedCountriesAndCurrenciesAsync(scope, cancellationToken);
            await SeedItemsAsync(scope, cancellationToken);
            await SeedInvoiesAsync(scope, cancellationToken);
            await SeedPlansAsync(scope, cancellationToken);
            await SeedSalesAsync(scope, cancellationToken);
        }
        catch(Exception ex)
        {
            Log.Error($"Data seeding failed: {ex.Message}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
