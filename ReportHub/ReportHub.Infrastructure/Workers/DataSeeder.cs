using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ReportHub.Infrastructure.Workers;

public partial class DataSeeder(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var scope = serviceProvider.CreateScope();
        
        await SeedClientsAsync(scope, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
