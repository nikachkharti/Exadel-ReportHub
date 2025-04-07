using System.Security.Claims;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using ReportHub.Identity.Configurations;
using ReportHub.Identity.Contexts;
using ReportHub.Identity.Models;
using ReportHub.Identity.Workers;

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

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseMongoDb()
            .UseDatabase(context.Database);
    })
    .AddServer(options =>
    { 
        options.SetIssuer(new Uri(authSettings.Issuer));
        options.SetTokenEndpointUris("connect/token")
            .AllowPasswordFlow()
            .AllowRefreshTokenFlow()
            .AllowClientCredentialsFlow();

        options.DisableAccessTokenEncryption();
        
        options.SetIntrospectionEndpointUris("/connect/introspect");

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
                    c.Claims["role"] = identity.FindFirst("role")?.Value;
                }
                
                return default;
            }));
        
        options.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();
        
        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(authSettings.AccessTokenLifeTimeMinutes));

        options.UseAspNetCore()
            .EnableTokenEndpointPassthrough();
    });

builder.Services.AddHostedService<OpenIddictClientSeeder>();
builder.Services.AddHostedService<DatabaseSeeder>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReportHub.Identity V1");
        c.RoutePrefix = "";
    });}

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