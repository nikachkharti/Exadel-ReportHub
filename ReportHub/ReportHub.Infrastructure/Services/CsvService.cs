using CsvHelper;
using ReportHub.Application.Contracts.FileContracts;
using System.Globalization;

namespace ReportHub.Infrastructure.Services;

public class CsvService : ICsvService
{
    public IAsyncEnumerable<T> ReadAllAsync<T>(Stream stream, CancellationToken cancellationToken) where T : class
    {
        var streamReader = new StreamReader(stream);
        var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

        return GetRecordsOneByOne<T>(csvReader);
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

    private static async IAsyncEnumerable<T> GetRecordsOneByOne<T>(CsvReader reader)
    {
        while (await reader.ReadAsync())
        {
            if (reader.TryGetField(0, out string field) 
                && field.Equals("All Statistics")) break;

            var record = reader.GetRecord<T>();

            if (record is null) yield break;

            yield return record!;
        }
    }
}
