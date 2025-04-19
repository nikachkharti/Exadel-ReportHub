using ReportHub.Application.Common.Models;

namespace ReportHub.Application.Contracts.CurrencyContracts;

public interface IExchangeCurrencyService
{
    Task<CurrencyDto> GetCurrencyAsync(string fromCurrency, string toCurrency);
    Task<CurrencyDto> GetCurrencyAsync(string fromCurrency, string toCurrency, DateTime date);
}
