using ReportHub.API.Extensions;
using Serilog;

namespace ReportHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                ContainerExtensions.ConfigureSerilog();

                Log.Information("Starting the application");

                var builder = WebApplication.CreateBuilder(args);

                builder.AddSerilog();
                builder.AddControllers();
                builder.AddSwagger();
                builder.AddMongoSerizliers();
                builder.AddInfrastructureLayer();
                builder.AddApplicationLayer();
                builder.AddOpenIddict();
                builder.AddAuthentication();
                builder.AddAuthorization();


                var app = builder.Build();

                app.UseDataSeeder();
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseHttpsRedirection();
                app.UseAuthentication();
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