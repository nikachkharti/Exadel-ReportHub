using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.DependencyInjection;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ReportHub.Infrastructure.Workers;

public partial class DataSeeder
{
    private async Task AddEcbSupport(IServiceScope scope, CancellationToken cancellationToken)
    {
        var currencyRepository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();

        var ecbSupportedCurrencyCodes = new HashSet<string>
    {
        "USD", "JPY", "BGN", "CZK", "DKK", "GBP", "HUF", "PLN", "RON", "SEK",
        "CHF", "ISK", "NOK", "RUB", "TRY", "AUD", "BRL", "CAD", "CNY", "HKD",
        "IDR", "ILS", "INR", "KRW", "MXN", "MYR", "NZD", "PHP", "SGD", "THB", "ZAR"
    };

        var updates = new Dictionary<Expression<Func<Currency, object>>, object>
    {
        { c => c.EcbSupport, true }
    };

        await currencyRepository.UpdateMultipleDocuments(
            c => ecbSupportedCurrencyCodes.Contains(c.Code),
            updates,
            cancellationToken
        );
    }

}
