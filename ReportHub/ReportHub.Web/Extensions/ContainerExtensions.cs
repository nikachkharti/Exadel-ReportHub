using Refit;
using ReportHub.Web.Components.Shared;
using ReportHub.Web.Services.Client;
using ReportHub.Web.Services.Invoice;
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
            var baseAddress = builder.Configuration.GetValue<string>("Refit:BaseAddress");

            builder.Services.AddRefitClient<IClientApi>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(baseAddress));

            builder.Services.AddRefitClient<IPlanApi>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(baseAddress));

            builder.Services.AddRefitClient<IInvoiceApi>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(baseAddress));
        }

        public static void AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();
        }

        public static void AddSharedStates(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<SelectedClientState>();
        }
    }
}
