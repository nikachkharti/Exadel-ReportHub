using ReportHub.Application.Contracts.CurrencyContracts;
using ReportHub.Application.Validators.Exceptions;
using System.Xml.Linq;

namespace ReportHub.Infrastructure.Services.CurrencyServices;

public class EcbCurrencyConverter : ICurrencyConverter
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EcbCurrencyConverter(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency, DateTime date)
    {
        var rates = await FetchRatesForDate(date);

        if (!rates.ContainsKey(fromCurrency))
            throw new UnsupportedCurrencyException(fromCurrency);

        if (!rates.ContainsKey(toCurrency))
            throw new UnsupportedCurrencyException(toCurrency);

        var eurToFrom = rates[fromCurrency];
        var eurToTo = rates[toCurrency];

        var eurAmount = amount / eurToFrom;
        return eurAmount * eurToTo;
    }

    private async Task<Dictionary<string, decimal>> FetchRatesForDate(DateTime date)
    {
        // ECB daily API only returns the latest available rates
        var url = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

        var client = _httpClientFactory.CreateClient();
        var response = await client.GetStringAsync(url);

        var xdoc = XDocument.Parse(response);
        var rates = new Dictionary<string, decimal>
        {
            { "EUR", 1m } 
        };

        var ns = xdoc.Root?.GetDefaultNamespace();
        var cubeRoot = xdoc.Descendants()
            .FirstOrDefault(x => x.Name.LocalName == "Cube" && x.Attribute("time") != null);

        if (cubeRoot != null)
        {
            foreach (var rateElement in cubeRoot.Elements())
            {
                var currency = rateElement.Attribute("currency")?.Value;
                var rateValue = rateElement.Attribute("rate")?.Value;

                if (!string.IsNullOrWhiteSpace(currency) && decimal.TryParse(rateValue, out var rate))
                {
                    rates[currency] = rate;
                }
            }
        }

        return rates;
    }
}
