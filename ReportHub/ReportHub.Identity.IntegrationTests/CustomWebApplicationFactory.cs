using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Validation;
using Polly;
using System.Net;
using System.Security.Claims;

namespace ReportHub.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
}
