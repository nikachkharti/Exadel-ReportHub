using System.Text.Json;
using ReportHub.Application.Common.Models;
using ReportHub.Application.Contracts.CurrencyContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace ReportHub.Infrastructure.Services.CurrencyServices;
public class ExchangeCurrencyService : IExchangeCurrencyService
{
    private readonly HttpClient _httpClient;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IMemoryCache _cache;         

    public ExchangeCurrencyService(
        HttpClient httpClient,
        ICurrencyRepository currencyRepository,
        IMemoryCache cache)                       
    {
        _httpClient = httpClient;
        _currencyRepository = currencyRepository;
        _cache = cache;
    }

    public Task<CurrencyDto> GetCurrencyAsync(string fromCurrency, string toCurrency)
        => GetCurrencyAsync(fromCurrency, toCurrency, DateTime.UtcNow);

    public async Task<CurrencyDto> GetCurrencyAsync(
        string fromCurrency,
        string toCurrency,
        DateTime date)
    {
        var key = $"{fromCurrency}_{toCurrency}_{date:yyyy-MM-dd}";

        if (_cache.TryGetValue<CurrencyDto>(key, out var cachedDto))
        {
            return cachedDto;                    
        }
        var dateStr = date.ToString("yyyy-MM-dd");
        var uri = $"{dateStr}?from={fromCurrency}&to={toCurrency}";
        var response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<CurrencyDto>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (dto != null)
        {
            var entity = new Currency
            {
                Base = dto.Base,
                Date = dto.Date,
                Rates = dto.Rates
            };
            await _currencyRepository.UpdateSingleDocument(
                x => x.Base == entity.Base && x.Date == entity.Date,
                entity,
                isUpsert: true);

            // Store in cache with expiration
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(15)
            };
            _cache.Set(key, dto, cacheOptions);
        }

        return dto;
    }
}
