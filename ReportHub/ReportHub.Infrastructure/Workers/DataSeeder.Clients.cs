using Microsoft.Extensions.DependencyInjection;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;

namespace ReportHub.Infrastructure.Workers;

public partial class DataSeeder
{
    private async Task SeedClientsAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var clientRepository = scope.ServiceProvider.GetRequiredService<IClientRepository>();

        var existingClients = await clientRepository.GetAll(pageNumber: 1, pageSize: 1);

        if (existingClients.Any()) return;
        
        var clients = GetClients();

        await clientRepository.InsertMultiple(clients, cancellationToken);
    }

    private static IEnumerable<Client> GetClients()
    {
        return new List<Client>()
        {
            new Client()
            {
                Id = "67fa2d8114e2389cd8064452",
                Name = "Alpha Soft",
                Specialization = "Software Development"
            },
            new Client()
            {
                Id = "67fa2d8114e2389cd8064453",
                Name = "Brick CO",
                Specialization = "Builiding and Development"
            },
            new Client()
            {
                Id = "67fa2d8114e2389cd8064454",
                Name = "Eduworld",
                Specialization = "Education and schoolarship"
            }
        };
    }
}
