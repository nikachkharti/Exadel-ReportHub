namespace ReportHub.Application.Validators.Exceptions
{
    public class UnsupportedCurrencyException : Exception
    {
        public string Currency { get; }

        public UnsupportedCurrencyException(string currency)
            : base($"Currency \"{currency}\" is not supported for conversion.")
        {
            Currency = currency;
        }

        public UnsupportedCurrencyException(string fromCurrency, string toCurrency)
            : base($"Conversion from \"{fromCurrency}\" to \"{toCurrency}\" is not supported.")
        {
            Currency = $"{fromCurrency}->{toCurrency}";
        }
    }
}
