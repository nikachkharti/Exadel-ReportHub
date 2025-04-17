namespace ReportHub.Application.Contracts.CurrencyContracts;

public interface ICurrencyService
{
    /// <summary>
    /// Gets the exchange rate from one currency to another on a specific date
    /// </summary>
    /// <param name="fromCurrency">The source currency code (e.g., "USD")</param>
    /// <param name="toCurrency">The target currency code (e.g., "EUR")</param>
    /// <param name="date">The date for which to get the exchange rate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The exchange rate as a decimal value</returns>
    Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency, DateTime date, CancellationToken cancellationToken);
}
