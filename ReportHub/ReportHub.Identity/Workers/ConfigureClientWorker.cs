using OpenIddict.Abstractions;

namespace ReportHub.Identity.Workers;

public class ConfigureClientWorker: IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public ConfigureClientWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        
        var appManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var appDescriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = "report-hub",
            ClientSecret = "client_secret_key",
            ClientType = OpenIddictConstants.ClientTypes.Confidential,
            DisplayName = "Report Hub API",
            RedirectUris =
            {
                new Uri("https://localhost:7153/callback")
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.GrantTypes.Password,
                OpenIddictConstants.Permissions.Prefixes.Scope + "report-hub-api",
            } 
        };
        
        var clientApp = await appManager.FindByClientIdAsync(appDescriptor.ClientId, cancellationToken);
        if (clientApp is null)
        {
            await appManager.CreateAsync(appDescriptor, cancellationToken);
        }
        else
        {
            await appManager.UpdateAsync(clientApp, appDescriptor, cancellationToken);
        }
        
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        var scopeDescriptor = new OpenIddictScopeDescriptor
        {
            Name = "report-hub-api",
            Resources =
            {
                "report-hub-resource_server"
            }
        };

        var identityScope = await scopeManager.FindByNameAsync(scopeDescriptor.Name, cancellationToken);
        if (identityScope is null)
        {
            await scopeManager.CreateAsync(scopeDescriptor, cancellationToken);
        }
        else
        {
            await scopeManager.UpdateAsync(identityScope, scopeDescriptor, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}