using NUnit.Framework;
using ReportHub.Application.Features.Clients.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ReportHub.IntegrationTests.StepDefinitions;
[Binding]
public class ClientCRUDSteps 
{
    private readonly HttpClient _client;
    private readonly FeatureContext _featureContext;
    private readonly ScenarioContext _scenarioContext;

    public ClientCRUDSteps(FeatureContext context, ScenarioContext scenarioContext)
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7153")
        };
        _featureContext = context;
        _scenarioContext = scenarioContext;
    }

    [Given("I am authorized as super admin")]
    public void GivenIAuthorizedWithValidSuperAdminRole()
    {
        var token = _featureContext["access_token"].ToString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [When(@"I create a client with name ""(.*)"" and specialization ""(.*)""")]
    public async Task WhenICreateClient(string name, string specialization)
    {
        var command = new
        {
            Name = name,
            Specialization = specialization
        };

        var response = await _client.PostAsJsonAsync("api/Clients", command);

        _featureContext["response"] = response;
    }

    [Then("the client should be created successfully")]
    public async Task ThenClientShouldBeCreated()
    {
        var response = (HttpResponseMessage)_featureContext["response"];

        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue();

        var json = JsonDocument.Parse(body);
        var clientId = json.RootElement.GetProperty("result").ToString();
        clientId.Should().NotBeNullOrEmpty();
        _featureContext["clientId"] = clientId!;
    }

    [Given("I am authorized as owner")]
    public void GivenIAuthorizedWithOwnerRole()
    {
        var token = _scenarioContext["access_token"].ToString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [When(@"I create a client with name ""(.*)"" and specialization ""(.*)"" as owner")]
    public async Task WhenICreateClientWithOwnerRole(string name, string specialization)
    {
        var command = new
        {
            Name = name,
            Specialization = specialization
        };

        var response = await _client.PostAsJsonAsync("api/Clients", command);

        _scenarioContext["response"] = response;
    }

    [Then("the response should be forbidden status code")]
    public async Task ThenTheResponseShouldBeForbiddenStatusCode()
    {
        var response = (HttpResponseMessage)_scenarioContext["response"];

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Given("I have created a client")]
    public void GivenCreatedClient()
    {
        var token = _featureContext["access_token"].ToString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [When(@"I request the client by ID")]
    public async Task WhenRequestClientById()
    {
        var clientId = _featureContext["clientId"].ToString();
        var response = await _client.GetAsync($"api/Clients/{clientId}");
        _featureContext["response"] = response;
    }

    [Then("the response should contain the client")]
    public async Task ThenResponseShouldContainClient()
    {
        var response = (HttpResponseMessage)_featureContext["response"];
        response.IsSuccessStatusCode.Should().BeTrue();

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        var conf = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var client = json.RootElement.GetProperty("result").Deserialize<ClientForGettingDto>(conf);

        Assert.That(client.Name.Equals("Acme Corp"));
    }

    [Given("I have created a client and got by id")]
    public void GivenClientAndGotById()
    {
        var token = _featureContext["access_token"].ToString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    
    [When("I delete the client")]
    public async Task WhenIDeleteClient()
    {
        var clientId = _featureContext["clientId"].ToString();
        var response = await _client.DeleteAsync($"api/Clients/{clientId}");
        _featureContext["response"] = response;
    }

    [Then("The client IsDeleted property should be true")]
    public async Task ThenClientShouldNotExist()
    {
        var clientId = _featureContext["clientId"].ToString();
        var response = await _client.GetAsync($"api/Clients/{clientId}");

        var body = await response.Content.ReadAsStringAsync();

        var json = JsonDocument.Parse(body);

        var conf = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var client = json.RootElement.GetProperty("result").Deserialize<ClientForGettingDto>(conf);

        client.Should().NotBeNull();
        client.IsDeleted.Should().BeTrue();
    }

}
