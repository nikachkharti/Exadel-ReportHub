using ReportHub.API.Middlewares;

namespace ReportHub.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseDataSeeder(this WebApplication app)
        {
            return app;
        }

        public static IApplicationBuilder UseExceptions(this WebApplication app)
        {
            return app.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
