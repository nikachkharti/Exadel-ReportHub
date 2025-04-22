using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using Serilog;
using System.Text.Json;

namespace ReportHub.Infrastructure.Workers;

public partial class DataSeeder
{
    private async Task SeedCountriesAndCurrenciesAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var countryRepository = scope.ServiceProvider.GetRequiredService<ICountryRepository>();
        var currencyRepository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();

        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();


        Log.Information("Fetching and seeding countries and currencies from REST Countries API...");

        var httpClient = httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync("https://restcountries.com/v3.1/all");

        if (!response.IsSuccessStatusCode)
        {
            Log.Warning("Failed to fetch data from REST Countries API");
            return;
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        var countriesData = JsonDocument.Parse(jsonString).RootElement;

        var countryEntities = new List<Country>();
        var currencyEntities = new List<Currency>();

        foreach (var country in countriesData.EnumerateArray())
        {
            var countryName = GetCountryName(country);

            var countryEntity = await AddCountryIfNotExist(countryRepository, countryEntities, countryName, cancellationToken);

            var currencies = GetCurrencies(country);

            foreach (var currency in currencies)
            {
                var currencyName = currency.Name;

                await AddCurrencyIfNotExist(currencyRepository, currencyEntities, countryEntity, currencyName, cancellationToken);
            }
        }

        await countryRepository.InsertMultiple(countryEntities, cancellationToken);

        await currencyRepository.InsertMultiple(currencyEntities, cancellationToken);

        Log.Information("Country and currency seeding completed");
    }

    private async Task AddCurrencyIfNotExist(ICurrencyRepository currencyRepository, List<Currency> currencyEntities, 
        Country countryEntity, string currencyName, CancellationToken cancellationToken)
    {
        var existingCurrency = await currencyRepository.Get(c => c.Code.Equals(currencyName), cancellationToken);

        if (existingCurrency is not null) return;

        currencyEntities.Add(new Currency()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Code = currencyName,
            CountryId = countryEntity.Id
        });
    }

    private async Task<Country> AddCountryIfNotExist(ICountryRepository countryRepository, List<Country> countryEntities,
        string countryName, CancellationToken cancellationToken)
    {
        var existingCountry = await countryRepository.Get(c => c.Name.Equals(countryName), cancellationToken);

        if (existingCountry is not null) return existingCountry;

        var countryEntity = new Country()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = countryName
        };

        countryEntities.Add(countryEntity);

        return countryEntity;
    }

    private IEnumerable<JsonProperty> GetCurrencies(JsonElement country)
    {
        return country
            .TryGetProperty("currencies", out var currencyProp)
            ? currencyProp.EnumerateObject()
            : Enumerable.Empty<JsonProperty>();
    }

    private string GetCountryName(JsonElement country)
    {
        return country
                    .GetProperty("name")
                    .GetProperty("common")
                    .GetString();
    }
}
