using CsvHelper;
using CsvHelper.Configuration;
using ReportHub.Application.Contracts.FileContracts;
using System.Globalization;

namespace ReportHub.Infrastructure.Services.FileServices;

public class CsvService : CsvBaseService, ICsvService
{
    public IAsyncEnumerable<T> ReadAllAsync<T>(Stream stream, CancellationToken cancellationToken) where T : class
    {
        var streamReader = new StreamReader(stream);
        var csvReader = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null
        });
        
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
    private async IAsyncEnumerable<T> GetRecordsOneByOne<T>(CsvReader reader)
    {
        var properties = GetTypeProperties<T>();

        await reader.ReadAsync();
        reader.ReadHeader();

        while (await reader.ReadAsync())
        {
            int count = GetAllExistPropertyInCsv(reader, properties);

            if (count != properties.Length) continue;

            var record = reader.GetRecord<T>();

            yield return record!;
        }
    }
}
