using OpenIddict.Abstractions;

namespace ReportHub.Identity.Workers;

public class ClientSeeder: IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public ClientSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        
        await SeedScopes(scope, cancellationToken);
        await SeedInternalApps(scope, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    
    private async Task SeedInternalApps(AsyncServiceScope scope, CancellationToken cancellationToken)
    {
        var appManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var appDescriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = "report-hub",
            ClientSecret = "client_secret_key",
            ClientType = OpenIddictConstants.ClientTypes.Confidential,
            DisplayName = "Report Hub API",
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Introspection,
                OpenIddictConstants.Permissions.Endpoints.Revocation,

                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.GrantTypes.Password,
                
                OpenIddictConstants.Permissions.Prefixes.Scope + "report-hub-api",
            } 
        };
        
        var client  = await appManager.FindByClientIdAsync(appDescriptor.ClientId, cancellationToken);
        if (client  is null)
        {
            await appManager.CreateAsync(appDescriptor, cancellationToken);
        }
        else
        {
            await appManager.UpdateAsync(client , appDescriptor, cancellationToken);
        }
    }

    private async Task SeedScopes(AsyncServiceScope scope, CancellationToken cancellationToken)
    {
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        var scopeDescriptor = new OpenIddictScopeDescriptor
        {
            Name = "report-hub-api",
            Resources =
            {
                "report-hub-resource_server"
            }
        };

        var scopeInstance = await scopeManager.FindByNameAsync(scopeDescriptor.Name, cancellationToken);
        if (scopeInstance is null)
        {
            await scopeManager.CreateAsync(scopeDescriptor, cancellationToken);
        }
        else
        {
            await scopeManager.UpdateAsync(scopeInstance, scopeDescriptor, cancellationToken);
        }    
    }
}