using Refit;
using ReportHub.Web.Components.Shared;
using ReportHub.Web.Services.Auth;
using ReportHub.Web.Services.Client;
using ReportHub.Web.Services.Plan;
using ReportHub.Web.Services.Refit;

namespace ReportHub.Web.Extensions
{
    public static class ContainerExtensions
    {
        public static void AddRazorComponents(this WebApplicationBuilder builder)
        {
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
        }

        public static void AddRefit(this WebApplicationBuilder builder)
        {
            var baseAddressApi = builder.Configuration.GetValue<string>("Refit:BaseAddressApi");
            var BaseAddressAuth = builder.Configuration.GetValue<string>("Refit:BaseAddressAuth");

            builder.Services.AddRefitClient<IClientApi>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(baseAddressApi));

            builder.Services.AddRefitClient<IPlanApi>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(baseAddressApi));

            builder.Services.AddRefitClient<IAuthApi>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(BaseAddressAuth));
        }

        public static void AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
        }

        public static void AddSharedStates(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<SelectedClientState>();
        }
    }
}
