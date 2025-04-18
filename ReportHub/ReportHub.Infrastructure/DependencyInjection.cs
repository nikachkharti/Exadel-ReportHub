using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReportHub.Infrastructure.Configurations;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Infrastructure.Repository;
using ReportHub.Infrastructure.Services.FileServices;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Infrastructure.Middleware;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Infrastructure.Services.IdentityServices;

namespace ReportHub.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure MongoDB settings
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDB"));

            // Register services
            services.AddScoped<ICsvService, CsvService>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IIdentityService, IdentityService>();

            // Register repositories
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IClientUserRepository, ClientUserRepository>();

            return services;
        }
    }
}
