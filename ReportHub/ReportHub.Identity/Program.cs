using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Validation.AspNetCore;
using ReportHub.Identity.Configurations;
using ReportHub.Identity.Contexts;
using ReportHub.Identity.Models;
using ReportHub.Identity.Workers;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ReportHub.Identity",
        Version = "v1"
    });

    c.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer` Generated-JWT-Token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    c
    .AddSecurityRequirement(
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
});

var authSettings = builder.Configuration.GetSection("Authentication").Get<AuthSettings>();
if (authSettings == null)
    throw new InvalidOperationException("Missing Authentication configuration");

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));

var mongoDbSettings = builder.Configuration.GetSection("MongoDb").Get<MongoDbSettings>();
if (mongoDbSettings == null)
    throw new InvalidOperationException("Missing MongoDbSettings configuration");

builder.Services.AddSingleton<IdentityDbContext>();

builder.Services.AddIdentityMongoDbProvider<User, Role, string>(identity =>
        {
            identity.Password.RequiredLength = 6;
            identity.Password.RequireDigit = false;
            identity.Password.RequireLowercase = false;
            identity.Password.RequireNonAlphanumeric = false;
            identity.User.RequireUniqueEmail = true;
        },
        mongo =>
        {
            mongo.ConnectionString = mongoDbSettings.ConnectionString;
        })
    .AddDefaultTokenProviders();

var provider = builder.Services.BuildServiceProvider();
var context = provider.GetRequiredService<IdentityDbContext>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});
builder.Services
    .AddOpenIddict()
    .AddCore(options =>
    {
        options.UseMongoDb()
            .UseDatabase(context.Database);
    })
    .AddServer(options =>
    {
        options.SetIssuer(new Uri(authSettings.Issuer));
        options.SetTokenEndpointUris("auth/login", "auth/refresh-token")
            .AllowPasswordFlow()
            .AllowRefreshTokenFlow()
            .AllowClientCredentialsFlow();

        options.DisableAccessTokenEncryption();

        options.SetIntrospectionEndpointUris("connect/introspect");

        options.RegisterScopes(
            OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Roles,
            OpenIddictConstants.Scopes.OfflineAccess,
            "report-hub-api-scope");

        options.AddEventHandler<OpenIddictServerEvents.HandleIntrospectionRequestContext>(b =>
            b.UseInlineHandler(c =>
            {
                if (c.Principal.Identity is ClaimsIdentity identity)
                {

                    c.Claims["UserId"] = identity.FindFirst("UserId")?.Value;
                    c.Claims["role"] = identity.FindAll("role").Select(r => r.Value).ToArray();
                }

                return default;
            }));

        options.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();

        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(authSettings.AccessTokenLifeTimeMinutes));

        options.UseAspNetCore()
            .EnableTokenEndpointPassthrough();
    })
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

builder.Services.AddHostedService<OpenIddictClientSeeder>();
builder.Services.AddHostedService<DatabaseSeeder>();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReportHub.Identity V1");
        c.RoutePrefix = "";
    });
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();