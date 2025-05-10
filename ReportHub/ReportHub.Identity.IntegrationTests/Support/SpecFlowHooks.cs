using ReportHub.Identity.Application.Features.UserClients.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace ReportHub.IntegrationTests.Support
{
    [Binding]
    public sealed class SpecFlowHooks
    {
        [BeforeFeature("ClientCRUD")]
        public static async Task BeforeFeatureAsync(FeatureContext featureContext)
        {
            string? clientToken = await Login("SuperAdmin");

            featureContext["access_token"] = clientToken;
        }

        [BeforeScenario("CreateClientFailure")]
        public static async Task BeforeCreateClientFailureScenario(ScenarioContext context)
        {
            var clientToken = await Login("Owner");

            context["access_token"] = clientToken;
        }

        private static async Task<string?> Login(string role)
        {
            HttpClient httpClient = GetClient();
            JsonDocument json = await GetLoginResponseDocument(httpClient);

            var accessToken = json.RootElement.GetProperty("access_token").GetString();
            var refreshToken = json.RootElement.GetProperty("refresh_token").GetString();

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var clientId = await GetUserClientId(role, httpClient);

            json = await GetActualTokenDocument(httpClient, json, refreshToken, clientId);

            var clientToken = json.RootElement.GetProperty("access_token").GetString();

            return clientToken;
        }

        private static async Task<JsonDocument> GetActualTokenDocument(HttpClient httpClient, JsonDocument json, string? refreshToken, string clientId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/auth/select-client")
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

            var response = await httpClient.SendAsync(request);

            var body = await response.Content.ReadAsStringAsync();

            json = JsonDocument.Parse(body);
            return json;
        }

        private static async Task<JsonDocument> GetLoginResponseDocument(HttpClient httpClient)
        {
            HttpRequestMessage request = GetRequestMessage();

            var response = await httpClient.SendAsync(request);

            var body = await response.Content.ReadAsStringAsync();

            var json = JsonDocument.Parse(body);

            return json;
        }

        private static async Task<string> GetUserClientId(string role, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync("/my-clients");

            var body = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var clients = JsonSerializer.Deserialize<IList<UserClientForGettingDto>>(body, option);

            var clientId = clients.First(x => x.Role.Equals(role)).Id;

            return clientId;
        }

        private static HttpRequestMessage GetRequestMessage()
        {
            return new HttpRequestMessage(HttpMethod.Post, "/auth/login")
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
        }

        private static HttpClient GetClient()
        {
            return new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7171"),
            };
        }
    }
}