using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;
using ReportHub.API.Extensions;
using ReportHub.Infrastructure.Configurations;
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
                builder.AddInfrastructureLayer();
                
                var authSettings = builder.Configuration.GetSection("Authentication").Get<AuthSettings>();
                if (authSettings == null)
                    throw new InvalidOperationException("Missing Authentication configuration");
                
                var key = builder.Configuration["OPEN_IDDICT_KEY"]
                                      ?? throw new InvalidOperationException("OPEN_IDDICT_KEY is not set");
                
                builder.Services.AddOpenIddict()
                    .AddValidation(options =>
                    {
                        options.SetIssuer(authSettings.Issuer);
                        options.AddEncryptionKey(new SymmetricSecurityKey(
                            Convert.FromBase64String(key)));

                        options.UseSystemNetHttp();
                        options.UseAspNetCore();
                    });
                
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://localhost:7171/";
                    options.Audience = "report-hub-resource-server";
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true
                    };
                });
                
                builder.Services.AddAuthorization();
                
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