using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

namespace Reporthub.Identity.SpecflowTests.StepDefinitions;

[Binding]
public class LoginSteps : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;
    private bool _isLoggedIn;
    public LoginSteps(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:7171") 
        });
    }

    [Given("the user is on the login page")]
    public void GivenTheUserIsOnTheLoginPage()
    {

    }

    [When(@"the user enters username ""(.*)"" and password ""(.*)""")]
    public async Task WhenTheUserEntersUsernameAndPassword(string username, string password)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/auth/login")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = "report-hub",
                ["client_secret"] = "client_secret_key",
                ["scope"] = "report-hub-api-scope roles offline_access",
                ["username"] = username,
                ["password"] = password
            })
        };

        var response = await _httpClient.SendAsync(request);

        var body = await response.Content.ReadAsStringAsync();

        _isLoggedIn = response.IsSuccessStatusCode;

    }

    [Then("the user should be logged in successfully")]
    public void ThenTheUserShouldBeLoggedInSuccessfully()
    {
        Assert.That(_isLoggedIn, "Login should succeed with valid credentials.");
    }
}
