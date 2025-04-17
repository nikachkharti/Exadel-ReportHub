using Microsoft.Extensions.Caching.Memory;
using ReportHub.Application.Contracts.CurrencyContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReportHub.Infrastructure.Services.CurrencyServices;

public class CurrencyService : ICurrencyService
{
    private readonly IMongoRepositoryBase<Currency> _mongoRepository;
    private readonly IMemoryCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private const string ECB_API_URL = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
    private const string ECB_HISTORICAL_API_URL = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist.xml";

    public CurrencyService(
        IMongoRepositoryBase<Currency> mongoRepository,
        IMemoryCache cache,
        IHttpClientFactory httpClientFactory)
    {
        _mongoRepository = mongoRepository;
        _cache = cache;
        _httpClientFactory = httpClientFactory;
        _logger = Log.ForContext<CurrencyService>();
    }

    public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency, DateTime date, CancellationToken cancellationToken)
    {
        // Early return if currencies are the same
        if (fromCurrency == toCurrency)
            return 1m;

        string cacheKey = $"ExchangeRate_{fromCurrency}_{toCurrency}_{date:yyyy-MM-dd}";

        // Try to get from cache first
        if (_cache.TryGetValue(cacheKey, out decimal cachedRate))
            return cachedRate;

        try
        {
            decimal rate;
            // All ECB rates are based on EUR
            if (fromCurrency == "EUR")
            {
                rate = await GetEcbRateAsync(toCurrency, date, cancellationToken);
            }
            else if (toCurrency == "EUR")
            {
                rate = 1 / await GetEcbRateAsync(fromCurrency, date, cancellationToken);
            }
            else
            {
                // Cross-currency conversion: fromCurrency -> EUR -> toCurrency
                decimal fromRate = await GetEcbRateAsync(fromCurrency, date, cancellationToken);
                decimal toRate = await GetEcbRateAsync(toCurrency, date, cancellationToken);
                rate = toRate / fromRate;
            }

            // Cache the result
            _cache.Set(cacheKey, rate, TimeSpan.FromHours(24));
            return rate;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting exchange rate from {FromCurrency} to {ToCurrency} on {Date}",
                fromCurrency, toCurrency, date);

            // If we can't get the current rate, try to get the latest available rate before that date
            return await GetLatestAvailableRateAsync(fromCurrency, toCurrency, date, cancellationToken);
        }
    }

    private async Task<decimal> GetEcbRateAsync(string currencyCode, DateTime date, CancellationToken cancellationToken)
    {
        string cacheKey = $"EcbRate_{currencyCode}_{date:yyyy-MM-dd}";

        if (_cache.TryGetValue(cacheKey, out decimal cachedRate))
            return cachedRate;

        try
        {
            // Use today's rates for current date
            if (date.Date >= DateTime.UtcNow.Date)
            {
                return await GetCurrentEcbRateAsync(currencyCode, cancellationToken);
            }

            // For historical dates, use the historical API
            return await GetHistoricalEcbRateAsync(currencyCode, date, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get ECB rate for {CurrencyCode} on {Date}", currencyCode, date);
            throw;
        }
    }

    private async Task<decimal> GetCurrentEcbRateAsync(string currencyCode, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetStringAsync(ECB_API_URL, cancellationToken);

        var xml = XDocument.Parse(response);
        var ns = xml.Root.GetDefaultNamespace();

        var rateElement = xml.Descendants(ns + "Cube")
            .Elements(ns + "Cube")
            .Elements(ns + "Cube")
            .FirstOrDefault(e => e.Attribute("currency")?.Value == currencyCode);

        if (rateElement == null || !decimal.TryParse(rateElement.Attribute("rate")?.Value, out decimal rate))
        {
            Log.Warning("Currency {CurrencyCode} not found in ECB exchange rates", currencyCode);
            throw new KeyNotFoundException($"Currency {currencyCode} not found in ECB exchange rates");
        }

        string cacheKey = $"EcbRate_{currencyCode}_{DateTime.UtcNow:yyyy-MM-dd}";
        _cache.Set(cacheKey, rate, TimeSpan.FromHours(24));

        return rate;
    }

    private async Task<decimal> GetHistoricalEcbRateAsync(string currencyCode, DateTime date, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetStringAsync(ECB_HISTORICAL_API_URL, cancellationToken);

        var xml = XDocument.Parse(response);
        var ns = xml.Root.GetDefaultNamespace();

        // Find the time element that matches our date
        var timeElement = xml.Descendants(ns + "Cube")
            .Elements(ns + "Cube")
            .FirstOrDefault(e => e.Attribute("time")?.Value == date.ToString("yyyy-MM-dd"));

        if (timeElement == null)
        {
            Log.Warning("No exchange rates found for date {Date}", date);
            throw new KeyNotFoundException($"No exchange rates found for date {date:yyyy-MM-dd}");
        }

        var rateElement = timeElement.Elements(ns + "Cube")
            .FirstOrDefault(e => e.Attribute("currency")?.Value == currencyCode);

        if (rateElement == null || !decimal.TryParse(rateElement.Attribute("rate")?.Value, out decimal rate))
        {
            Log.Warning("Currency {CurrencyCode} not found in ECB exchange rates on {Date}", currencyCode, date);
            throw new KeyNotFoundException($"Currency {currencyCode} not found in ECB exchange rates for {date:yyyy-MM-dd}");
        }

        string cacheKey = $"EcbRate_{currencyCode}_{date:yyyy-MM-dd}";
        _cache.Set(cacheKey, rate, TimeSpan.FromHours(24));

        return rate;
    }

    private async Task<decimal> GetLatestAvailableRateAsync(string fromCurrency, string toCurrency, DateTime date, CancellationToken cancellationToken)
    {
        Log.Information("Trying to find latest available exchange rate before {Date}", date);

        // Try up to 30 days before the requested date
        for (int i = 1; i <= 30; i++)
        {
            var previousDate = date.AddDays(-i);
            try
            {
                var rate = await GetExchangeRateAsync(fromCurrency, toCurrency, previousDate, cancellationToken);
                Log.Information("Found exchange rate from {PreviousDate}", previousDate);
                return rate;
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "No exchange rate found for {PreviousDate}", previousDate);
                // Continue trying earlier dates
            }
        }

        Log.Error("Could not find any exchange rate for {FromCurrency} to {ToCurrency} within 30 days before {Date}",
            fromCurrency, toCurrency, date);
        throw new Exception($"No exchange rate data available for {fromCurrency} to {toCurrency}");
    }
}