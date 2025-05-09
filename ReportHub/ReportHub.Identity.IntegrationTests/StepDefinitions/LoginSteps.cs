using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using ReportHub.Identity.Application.Features.UserClients.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace Reporthub.Identity.SpecflowTests.StepDefinitions
{
    [Binding]
    public class LoginSteps : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly FeatureContext _context;
        private string _username;
        private string _password;
        private string _clientId;

        public LoginSteps(CustomWebApplicationFactory factory, FeatureContext context)
        {
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost:7171"),
            });
            _context = context;
        }

        [Given(@"I have credentials ""(.*)"" and ""(.*)""")]
        public void GivenIHaveCredentials(string username, string password)
        {
            _username = username;
            _password = password;
        }

        [When("I send a login request")]
        public async Task WhenISendLoginRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/auth/login")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "password",
                    ["client_id"] = "report-hub",
                    ["client_secret"] = "client_secret_key",
                    ["scope"] = "report-hub-api-scope roles offline_access",
                    ["username"] = _username,
                    ["password"] = _password
                })
            };

            var response = await _httpClient.SendAsync(request);
            _context["response"] = response;
        }

        [Then("I receive an access token")]
        public async Task ThenIReceiveAccessTokenAsync()
        {
            var response = (HttpResponseMessage)_context["response"]!;

            var body = await response.Content.ReadAsStringAsync();

            response.IsSuccessStatusCode.Should().BeTrue();

            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var accessToken = json.RootElement.GetProperty("access_token").GetString();
            var refreshToken = json.RootElement.GetProperty("refresh_token").GetString();

            accessToken.Should().NotBeNullOrEmpty();
            refreshToken.Should().NotBeNullOrEmpty();

            _context["access_token"] = accessToken!;
            _context["refresh_token"] = refreshToken!;
        }

        [Given(@"I have valid credentials and I have received an access token")]
        public void GivenIHaveValidCredentialsAndToken()
        {
            Assert.That(_context.ContainsKey("access_token"));
        }

        [When("I request the list of my clients")]
        public async Task WhenIRequestClientsAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _context["access_token"].ToString());

            var response = await _httpClient.GetAsync("/my-clients");
            _context["response"] = response;
        }

        [Then("I receive a list of clients")]
        public async Task ThenIReceiveClientListAsync()
        {
            var response = (HttpResponseMessage)_context["response"]!;
            
            response.IsSuccessStatusCode.Should().BeTrue();

            var body = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var clients = JsonSerializer.Deserialize<IList<UserClientForGettingDto>>(body, option);

            clients.Should().NotBeNull("Should");

            clients.Should().HaveCountGreaterThan(0);

            // Take the first client id for next step
            _clientId = clients.Last().Id;
            _context["client_id"] = _clientId;
        }

        [Given(@"I have valid credentials, I have received an access token, and I have a client ID")]
        public void GivenIHaveValidCredentialsWithClient()
        {
            // Ensure all prior steps have been executed successfully
            _context.ContainsKey("access_token").Should().BeTrue();
            _context.ContainsKey("client_id").Should().BeTrue();

            _httpClient.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue("Bearer", _context["access_token"].ToString());
        }

        [When("I send a switch-context request")]
        public async Task WhenSendSwitchContext()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/auth/select-client")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "refresh_token",
                    ["client_id"] = "report-hub",
                    ["client_secret"] = "client_secret_key",
                    ["scope"] = "report-hub-api-scope roles offline_access",
                    ["refresh_token"] = _context["refresh_token"].ToString()!,
                    ["userClientId"] = _context["client_id"].ToString()!
                })
            };

            var response = await _httpClient.SendAsync(request);
            _context["response"] = response;
        }

        [Then("I receive a token scoped to that client")]
        public async Task ThenReceiveScopedToken()
        {
            var response = (HttpResponseMessage)_context["response"]!;
            
            response.IsSuccessStatusCode.Should().BeTrue();

            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var clientToken = json.RootElement.GetProperty("access_token").GetString();

            clientToken.Should().NotBeNullOrEmpty();
        }
    }
}
