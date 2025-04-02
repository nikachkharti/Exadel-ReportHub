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

        return await GetRecordsAsList(records);
    }


    public async Task<Stream> WriteAllAsync<T>(IEnumerable<T> datas, IReadOnlyDictionary<string, object> statistics, CancellationToken token)
    {
        var memoryStream = new MemoryStream();
        var streamWriter = new StreamWriter(memoryStream);
        var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

        csvWriter.WriteRecords(datas);

        WriteStatistics(statistics, csvWriter);

        await streamWriter.FlushAsync(token);

        memoryStream.Position = 0;

        return memoryStream;
    }

    private static void WriteStatistics(IReadOnlyDictionary<string, object> statistics, CsvWriter csvWriter)
    {
        csvWriter.NextRecord();

        foreach (var (key, value) in statistics)
        {
            csvWriter.WriteField(key);
            csvWriter.WriteField(value);
            csvWriter.NextRecord();
        }
    }

    private static async Task<IEnumerable<T>> GetRecordsAsList<T>(IAsyncEnumerable<T> records) where T : class
    {
        var result = new List<T>();
        await foreach (var record in records)
        {
            if (record is null) break;

            result.Add(record);
        }

        return result;
    }
}
