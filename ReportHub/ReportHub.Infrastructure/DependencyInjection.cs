using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReportHub.Application.Contracts;
using ReportHub.Infrastructure.Helper;
using ReportHub.Infrastructure.Repository;

namespace ReportHub.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure MongoDB settings
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDB"));

            // Register repositories
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            return services;
        }
    }
}
