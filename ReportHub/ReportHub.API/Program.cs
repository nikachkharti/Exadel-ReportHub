using ReportHub.API.Extensions;

namespace ReportHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.AddControllers();
            //builder.AddOpenApi();
            builder.AddSwagger();
            builder.AddInfrastructureLayer();


            var app = builder.Build();

            //app.MapOpenApi();
            app.UseDataSeeder();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
