using Microsoft.Extensions.Hosting;

namespace ReportHub.Infrastructure.Workers;

public class DataSeeder(IServiceProvider serviceProvider) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
