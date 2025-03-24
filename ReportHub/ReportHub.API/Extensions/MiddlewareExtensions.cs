using ReportHub.Infrastructure.Middleware;

namespace ReportHub.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseDataSeeder(this WebApplication app)
        {
            return app.UseMiddleware<DataSeedingMiddleware>();
        }
    }
}
