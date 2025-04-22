using ReportHub.API.Extensions;
using Serilog;

namespace ReportHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ContainerExtensions.ConfigureSerilog();

            var builder = WebApplication.CreateBuilder(args);

            builder.AddSerilog();
            builder.AddControllers();
            builder.AddSwagger();
            builder.AddInfrastructureLayer();
            builder.AddApplicationLayer();
            builder.AddOpenIddict();
            builder.AddAuthentication();
            builder.AddAuthorization();



            var app = builder.Build();

            app.UseDataSeeder();
            app.UseSerilogRequestLogging();
            app.UseExceptions();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}