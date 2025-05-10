using NUnit.Framework;
using ReportHub.Application.Features.Clients.DTOs;
using ReportHub.Application.Features.Customers.Commands;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ReportHub.Identity.IntegrationTests.StepDefinitions;

[Binding]
public class CustomerCRUDSteps
{
    private readonly HttpClient _client;
    private readonly FeatureContext _featureContext;
    private readonly ScenarioContext _scenarioContext;

    public CustomerCRUDSteps(FeatureContext context, ScenarioContext scenarioContext)
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7153")
        };
        _featureContext = context;
        _scenarioContext = scenarioContext;
    }

    [Given("I am authorized as owner")]
    public void GivenIAuthorizedAsOwner()
    {
        var token = _featureContext["access_token"].ToString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [When(@"I create a customer with name ""(.*)"", email ""(.*)"", and country ID ""(.*)""")]
    public async Task WhenICreateCustomer(string name, string email, string countryId)
    {
        var command = new CreateCustomerCommand(name, email, countryId);

        var response = await _client.PostAsJsonAsync("api/Customers", command);

        _featureContext["response"] = response;
    }

    [Then("the customer should be created successfully")]
    public async Task ThenCustomerShouldBeCreated()
    {
        var response = (HttpResponseMessage)_featureContext["response"];

        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue();

        var json = JsonDocument.Parse(body);
        var customerId = json.RootElement.GetProperty("result").ToString();
        customerId.Should().NotBeNullOrEmpty();
        _featureContext["customerId"] = customerId!;
    }

    [Given("I am authorized as super admin")]
    public void GivenIAuthorizedWithOwnerRole()
    {
        var token = _scenarioContext["access_token"].ToString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [When(@"I create a customer with name ""(.*)"", email ""(.*)"", and country ID ""(.*)"" as super admin")]
    public async Task WhenICreateCustomerAsSuperAdmin(string name, string email, string countryId)
    {
        var command = new CreateCustomerCommand(name, email, countryId);


        var response = await _client.PostAsJsonAsync("api/Customers", command);

        _scenarioContext["response"] = response;
    }

    [Then("the response should be forbidden status code")]
    public void ThenTheResponseShouldBeForbiddenStatusCode()
    {
        var response = (HttpResponseMessage)_scenarioContext["response"];

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Given("I have created a customer")]
    public void GivenICreatedCustomer()
    {
        var token = _featureContext["access_token"].ToString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [When(@"I request the customer by ID")]
    public async Task WhenIRequestCustomerById()
    {
        var customerId = _featureContext["customerId"].ToString();
        var response = await _client.GetAsync($"api/Customers/{customerId}");
        _featureContext["response"] = response;
    }

    [Then("the response should contain the customer")]
    public async Task ThenResponseShouldContainCustomer()
    {
        var response = (HttpResponseMessage)_featureContext["response"];
        response.IsSuccessStatusCode.Should().BeTrue();

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        var conf = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var customer = json.RootElement.GetProperty("result").Deserialize<ClientForGettingDto>(conf);

        Assert.That(customer.Name.Equals("test"));
    }

    [Given("I have created a customer and got by id")]
    public void GivenICreatedCustomerAndGotById()
    {
        var token = _featureContext["access_token"].ToString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [When("I delete the customer")]
    public async Task WhenIDeleteCustomer()
    {
        var customerId = _featureContext["customerId"].ToString();
        var response = await _client.DeleteAsync($"api/Customers/{customerId}");
        _featureContext["response"] = response;
    }

    [Then("The customer IsDeleted property should be true")]
    public async Task ThenCustomerIsDeletedPropertyShouldBeTrue()
    {
        var customerId = _featureContext["customerId"].ToString();
        var response = await _client.GetAsync($"api/Customers/{customerId}");

        var body = await response.Content.ReadAsStringAsync();

        var json = JsonDocument.Parse(body);

        var conf = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var customer = json.RootElement.GetProperty("result").Deserialize<ClientForGettingDto>(conf);

        customer.Should().NotBeNull();
        customer.IsDeleted.Should().BeTrue();
    }
}
