using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using ReportHub.Application.Extensions;
using ReportHub.Infrastructure;
using ReportHub.Infrastructure.Configurations;
using Serilog;
using ReportHub.API.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using ReportHub.API.Authorization.Handlers;
using ReportHub.API.Authorization.Permissions;
using System.Text.Json.Serialization;

namespace ReportHub.API.Extensions
{
    public static class ContainerExtensions
    {
        public static void AddControllers(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
        }

        public static void AddSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer Authorization string as following: `Bearer` Generated-JWT-Token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                options.AddSecurityRequirement(
                        new OpenApiSecurityRequirement()
                        {
                            {
                                new OpenApiSecurityScheme()
                                {
                                    Reference = new OpenApiReference()
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = JwtBearerDefaults.AuthenticationScheme
                                    }
                                },
                                new string[]{}
                            }
                        }
                );


                #region XML documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; //ReportHub.API.xml
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
                #endregion

            });
        }

        public static void AddInfrastructureLayer(this WebApplicationBuilder builder)
        {
            builder.Services.AddInfrastructure(builder.Configuration);
        }

        public static void ConfigureSerilog()
        {
            // Create logger configuration before builder
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build())
                .CreateLogger();
        }

        public static void AddSerilog(this WebApplicationBuilder builder)
        {
            // Replace default logging with Serilog
            builder.Logging.ClearProviders();
            builder.Host.UseSerilog(); // This is crucial
        }

        public static void AddApplicationLayer(this WebApplicationBuilder builder)
        {
            builder.Services.AddApplication();
        }

        public static void AddOpenIddict(this WebApplicationBuilder builder)
        {
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
        }

        public static void AddAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        }

        public static void AddAuthorization(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization(options =>
            {
                foreach (var permission in Enum.GetValues<PermissionType>())
                {
                    options.AddPolicy(permission.ToString(), policy =>
                    {
                        policy.Requirements.Add(new PermissionRequirement(permission));
                    });
                }
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionHandlers>();
        }
    }
}
