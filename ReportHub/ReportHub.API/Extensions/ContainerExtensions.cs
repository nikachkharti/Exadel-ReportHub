using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using ReportHub.Infrastructure;
using System.Reflection;

namespace ReportHub.API.Extensions
{
    public static class ContainerExtensions
    {
        public static void AddControllers(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
        }

        public static void AddOpenApi(this WebApplicationBuilder builder)
        {
            builder.Services.AddOpenApi();
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
    }
}
