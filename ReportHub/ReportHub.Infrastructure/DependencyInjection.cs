using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ReportHub.Application.Contracts;
using ReportHub.Infrastructure.Repository;

namespace ReportHub.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure MongoDB settings from appsettings.json
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDB"));

            // Register MongoDB Client
            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            // Register MongoDB Database
            services.AddSingleton(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                return client.GetDatabase(settings.DatabaseName);
            });

            // Register Repositories
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            return services;
        }
    }
}
