using ReportHub.Identity.Application.Features.UserClients.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace ReportHub.IntegrationTests.Support
{
    [Binding]
    public sealed class SpecFlowHooks : IClassFixture<CustomWebApplicationFactory>
    {

        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks
        //private  FeatureContext _featureContext;
        //private  HttpClient httpClient;
        //public SpecFlowHooks(FeatureContext featureContext, CustomWebApplicationFactory factory)
        //{
        //    httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        //    {
        //        BaseAddress = new Uri("https://localhost:7171"),
        //    });

        //    _featureContext = featureContext;
        //}

        [BeforeFeature("ClientCRUD")]
        public static async Task BeforeFeatureAsync(FeatureContext featureContext)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7171"),
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/auth/login")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "password",
                    ["client_id"] = "report-hub",
                    ["client_secret"] = "client_secret_key",
                    ["scope"] = "report-hub-api-scope roles offline_access",
                    ["username"] = "admin@example.com",
                    ["password"] = "Admin123$"
                })
            };

            var response = await httpClient.SendAsync(request);

            var body = await response.Content.ReadAsStringAsync();

            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var accessToken = json.RootElement.GetProperty("access_token").GetString();
            var refreshToken = json.RootElement.GetProperty("refresh_token").GetString();

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            response = await httpClient.GetAsync("/my-clients");

            body = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var clients = JsonSerializer.Deserialize<IList<UserClientForGettingDto>>(body, option);

            var clientId = clients.First(x => x.ClientId is null).Id;

            request = new HttpRequestMessage(HttpMethod.Post, "/auth/select-client")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "refresh_token",
                    ["client_id"] = "report-hub",
                    ["client_secret"] = "client_secret_key",
                    ["scope"] = "report-hub-api-scope roles offline_access",
                    ["refresh_token"] = refreshToken,
                    ["userClientId"] = clientId
                })
            };

            response = await httpClient.SendAsync(request);

            body = await response.Content.ReadAsStringAsync();

            json = JsonDocument.Parse(body);
            var clientToken = json.RootElement.GetProperty("access_token").GetString();


            featureContext["access_token"] = clientToken;
        }

        [BeforeScenario("CreateClientFailure")]
        public static async Task BeforeCreateClientFailureScenario(ScenarioContext context)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7171"),
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/auth/login")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "password",
                    ["client_id"] = "report-hub",
                    ["client_secret"] = "client_secret_key",
                    ["scope"] = "report-hub-api-scope roles offline_access",
                    ["username"] = "admin@example.com",
                    ["password"] = "Admin123$"
                })
            };

            var response = await httpClient.SendAsync(request);

            var body = await response.Content.ReadAsStringAsync();

            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var accessToken = json.RootElement.GetProperty("access_token").GetString();
            var refreshToken = json.RootElement.GetProperty("refresh_token").GetString();

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            response = await httpClient.GetAsync("/my-clients");

            body = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var clients = JsonSerializer.Deserialize<IList<UserClientForGettingDto>>(body, option);

            var clientId = clients.First(x => x.Role.Equals("Owner")).Id;

            request = new HttpRequestMessage(HttpMethod.Post, "/auth/select-client")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "refresh_token",
                    ["client_id"] = "report-hub",
                    ["client_secret"] = "client_secret_key",
                    ["scope"] = "report-hub-api-scope roles offline_access",
                    ["refresh_token"] = refreshToken,
                    ["userClientId"] = clientId
                })
            };

            response = await httpClient.SendAsync(request);

            body = await response.Content.ReadAsStringAsync();

            json = JsonDocument.Parse(body);
            var clientToken = json.RootElement.GetProperty("access_token").GetString();


            context["access_token"] = clientToken;
        }
    }
}