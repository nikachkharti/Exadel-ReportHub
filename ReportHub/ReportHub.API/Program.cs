using OpenIddict.Abstractions;
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
                builder.AddApplicationLayer();
                
                var authSettings = builder.Configuration.GetSection("Authentication").Get<AuthSettings>();
                if (authSettings == null)
                    throw new InvalidOperationException("Missing Authentication configuration");
                
                builder.Services.AddOpenIddict()
                    .AddValidation(options =>
                    {
                        options.SetIssuer(authSettings.Issuer);
                        options.AddAudiences("report-hub-api-audience");
        
                        options.UseIntrospection()
                            .SetClientId("report-hub")
                            .SetClientSecret("client_secret_key");
                        
                        options.UseSystemNetHttp();
                        options.UseAspNetCore();
                        
                        options.Configure(opts =>
                        {
                            opts.TokenValidationParameters.RoleClaimType = OpenIddictConstants.Claims.Role;
                        });
                    });

                builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
                
                builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy("admin", policy =>
                        policy.RequireRole("admin"));
                    options.AddPolicy("user", policy =>
                        policy.RequireRole("user"));
                });
                
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