using NUnit.Framework;
using ReportHub.Application.Features.Clients.DTOs;
using ReportHub.Application.Features.Items.Commands;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ReportHub.Identity.IntegrationTests.StepDefinitions;

[Binding]
class ItemCRUDSteps
{
    private readonly HttpClient _client;
    private readonly FeatureContext _featureContext;
    private readonly ScenarioContext _scenarioContext;

    public ItemCRUDSteps(FeatureContext context, ScenarioContext scenarioContext)
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7153")
        };
        _featureContext = context;
        _scenarioContext = scenarioContext;
    }

    [Given("I am authorized as owner for item crud")]
    public void GivenIAuthorizedAsOwnerForItemCRUD()
    {
        var token = _featureContext["access_token"].ToString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [When(@"I create a item with name ""(.*)"", description ""(.*)"", price ""(.*)"", and currency ""(.*)""")]
    public async Task WhenICreateItem(string name, string description, string price, string currency)
    {
        var command = new CreateItemCommand(name, description, decimal.Parse(price), currency);

        var response = await _client.PostAsJsonAsync("api/Items", command);

        _featureContext["response"] = response;
    }

    [Then("the item should be created successfully")]
    public async Task ThenItemShouldBeCreated()
    {
        var response = (HttpResponseMessage)_featureContext["response"];

        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue();

        var json = JsonDocument.Parse(body);
        var itemId = json.RootElement.GetProperty("result").ToString();
        itemId.Should().NotBeNullOrEmpty();
        _featureContext["itemId"] = itemId!;
    }

    [Given("I am authorized as super admin for item crud")]
    public void GivenIAuthorizedAsSuperAdminForItemCRUD()
    {
        var token = _scenarioContext["access_token"].ToString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [When(@"I create a item with name ""(.*)"", description ""(.*)"", price ""(.*)"", and currency ""(.*)"" as super admin")]
    public async Task WhenICreateItemAsSuperAdmin(string name, string description, string price, string currency)
    {
        var command = new CreateItemCommand(name, description, decimal.Parse(price), currency);


        var response = await _client.PostAsJsonAsync("api/Items", command);

        _scenarioContext["response"] = response;
    }

    [Then("the response should be forbidden status code for item creation")]
    public void ThenTheResponseShouldBeForbiddenStatusCodeForItemCreation()
    {
        var response = (HttpResponseMessage)_scenarioContext["response"];

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Given("I have created a item")]
    public void GivenICreatedItem()
    {
        var token = _featureContext["access_token"].ToString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [When(@"I request the item by ID")]
    public async Task WhenIRequestItemById()
    {
        var itemId = _featureContext["itemId"].ToString();
        var response = await _client.GetAsync($"api/Items/{itemId}");
        _featureContext["response"] = response;
    }

    [Then("the response should contain the item")]
    public async Task ThenResponseShouldContainItem()
    {
        var response = (HttpResponseMessage)_featureContext["response"];
        response.IsSuccessStatusCode.Should().BeTrue();

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        var conf = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var item = json.RootElement.GetProperty("result").Deserialize<ClientForGettingDto>(conf);

        Assert.That(item.Name.Equals("test"));
    }

    [Given("I have created a item and got by id")]
    public void GivenICreatedItemAndGotById()
    {
        var token = _featureContext["access_token"].ToString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [When("I delete the item")]
    public async Task WhenIDeleteItem()
    {
        var itemId = _featureContext["itemId"].ToString();
        var response = await _client.DeleteAsync($"api/Items/{itemId}");
        _featureContext["response"] = response;
    }

    [Then("The item IsDeleted property should be true")]
    public async Task ThenItemIsDeletedPropertyShouldBeTrue()
    {
        var itemId = _featureContext["itemId"].ToString();
        var response = await _client.GetAsync($"api/Items/{itemId}");

        var body = await response.Content.ReadAsStringAsync();

        var json = JsonDocument.Parse(body);

        var conf = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var item = json.RootElement.GetProperty("result").Deserialize<ClientForGettingDto>(conf);

        item.Should().NotBeNull();
        item.IsDeleted.Should().BeTrue();
    }
}
