namespace ReportHub.Application.Contracts.CurrencyContracts;

public interface ICurrencyConverter
{
    Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency, DateTime date);
}
