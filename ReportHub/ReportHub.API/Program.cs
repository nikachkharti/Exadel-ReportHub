using ReportHub.API.Extensions;
using Serilog;

namespace ReportHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create logger configuration before builder
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build())
                .CreateLogger();

            try
            {
                Log.Information("Starting the application");

                var builder = WebApplication.CreateBuilder(args);

                // Replace default logging with Serilog
                builder.Logging.ClearProviders();
                builder.Host.UseSerilog(); // This is crucial

                builder.AddControllers();
                builder.AddSwagger();
                builder.AddInfrastructureLayer();

                var app = builder.Build();

                app.UseDataSeeder();
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                Log.Information("Application is running");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}