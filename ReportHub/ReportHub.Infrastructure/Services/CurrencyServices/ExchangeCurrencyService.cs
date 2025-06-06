﻿using System.Text.Json;
using ReportHub.Application.Contracts.CurrencyContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;

namespace ReportHub.Infrastructure.Services.CurrencyServices;
public class ExchangeCurrencyService : IExchangeCurrencyService
{
    private readonly HttpClient _httpClient;
    private readonly IExchangeRateRepository _currencyRepository;
    private readonly IMemoryCache _cache;

    public ExchangeCurrencyService(
        HttpClient httpClient,
        IExchangeRateRepository currencyRepository,
        IMemoryCache cache)
    {
        _httpClient = httpClient;
        _currencyRepository = currencyRepository;
        _cache = cache;
    }

    public Task<decimal> GetCurrencyAsync(string fromCurrency, string toCurrency)
        => GetCurrencyAsync(fromCurrency, toCurrency, DateTime.UtcNow);

    public async Task<decimal> GetCurrencyAsync(
        string fromCurrency,
        string toCurrency,
        DateTime date)
    {
        var key = $"{fromCurrency}_{toCurrency}_{date:yyyy-MM-dd}";

        if (_cache.TryGetValue<decimal>(key, out var cachedDto))
        {
            return cachedDto;
        }
        var something = new HttpClient();
        var dateStr = date.ToString("yyyy-MM-dd");
        var uri = $"https://open.er-api.com/v6/latest/{fromCurrency}";
        var response = await something.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        
        string json = await response.Content.ReadAsStringAsync();
        var data = JsonDocument.Parse(json);

        var rate = data.RootElement
                      .GetProperty("rates")
                      .GetProperty(toCurrency)
                      .GetDecimal();
        // Logic If we want to save Exchange Rate
        //if (dto != null)
        //{
        //    var entity = new ExchangeRate
        //    {
        //        Base = dto.Base,
        //        Date = dto.Date,
        //        Rates = dto.Rates
        //    };
        //    await _currencyRepository.UpdateSingleDocument(
        //        x => x.Base == entity.Base && x.Date == entity.Date,
        //        entity,
        //        isUpsert: true);

       var cacheOptions = new MemoryCacheEntryOptions
       {
           AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
           SlidingExpiration = TimeSpan.FromMinutes(15)
       };
        _cache.Set(key, rate, cacheOptions);
        return rate;
    }
}
