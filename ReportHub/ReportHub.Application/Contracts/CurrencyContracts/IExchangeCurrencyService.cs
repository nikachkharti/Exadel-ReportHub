using ReportHub.Application.Common.Models;

namespace ReportHub.Application.Contracts.CurrencyContracts;

public interface IExchangeCurrencyService
{
    Task<decimal> GetCurrencyAsync(string fromCurrency, string toCurrency);
    Task<decimal> GetCurrencyAsync(string fromCurrency, string toCurrency, DateTime date);
}
