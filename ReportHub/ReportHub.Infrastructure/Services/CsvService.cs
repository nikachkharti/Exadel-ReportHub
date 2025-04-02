using ReportHub.Application.Contracts.FileContracts;

namespace ReportHub.Infrastructure.Services;

public class CsvService : ICsvService
{
    public Task<IEnumerable<T>> ReadAllAsync<T>(Stream stream, CancellationToken cancellationToken) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<Stream> WriteAllAsync<T>(IEnumerable<T> datas, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
