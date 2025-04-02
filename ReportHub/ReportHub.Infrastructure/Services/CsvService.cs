using CsvHelper;
using CsvHelper.Configuration;
using ReportHub.Application.Contracts.FileContracts;
using System.Globalization;

namespace ReportHub.Infrastructure.Services;

public class CsvService : ICsvService
{
    public async Task<IEnumerable<T>> ReadAllAsync<T>(Stream stream, CancellationToken cancellationToken) where T : class
    {
        var streamReader = new StreamReader(stream);
        var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
        var records = csvReader.GetRecordsAsync<T>(cancellationToken);

        var result = new List<T>();
        await foreach(var record in records)
        {
            if (record is null) break;

            result.Add(record);   
        }

        return result;
    }

    public Task<Stream> WriteAllAsync<T>(IEnumerable<T> datas, IReadOnlyDictionary<string, object> statistics, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
