using System.Reflection;
using System.Runtime.CompilerServices;
using ClosedXML.Excel;
using ReportHub.Application.Contracts.FileContracts;

namespace ReportHub.Infrastructure.Services.FileServices;

public class ExcelService : IExcelService
{
    public IAsyncEnumerable<T> ReadAllAsync<T>(Stream stream, CancellationToken cancellationToken) where T : class
    {
        return GetRecordsOneByOne<T>(stream, cancellationToken);
    }

    public async Task<Stream> WriteAllAsync<T>(IEnumerable<T> datas, IReadOnlyDictionary<string, object> statistics, CancellationToken token)
    {
        var memoryStream = new MemoryStream();
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Data");
        var properties = typeof(T).GetProperties();

        WriteHeaders(worksheet, properties);
        WriteData(worksheet, datas, properties);
        WriteStatistics(statistics, workbook);

        worksheet.Columns().AdjustToContents();
        workbook.SaveAs(memoryStream);
        memoryStream.Position = 0;
        return memoryStream;
    }

    private static void WriteHeaders(IXLWorksheet worksheet, PropertyInfo[] properties)
    {
        for (int i = 0; i < properties.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = properties[i].Name;
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
        }
    }

    private static void WriteData<T>(IXLWorksheet worksheet, IEnumerable<T> datas, PropertyInfo[] properties)
    {
        int row = 2;
        foreach (var item in datas)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cell(row, i + 1).Value = properties[i].GetValue(item)?.ToString();
            }
            row++;
        }
    }

    private static void WriteStatistics(IReadOnlyDictionary<string, object> statistics, XLWorkbook workbook)
    {
        if (statistics.Count > 0)
        {
            var statsSheet = workbook.Worksheets.Add("Statistics");
            int row = 1;

            foreach (var (key, value) in statistics)
            {
                statsSheet.Cell(row, 1).Value = key;
                statsSheet.Cell(row, 2).Value = value?.ToString();
                row++;
            }
        }
    }

    private async IAsyncEnumerable<T> GetRecordsOneByOne<T>(Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken) where T : class
    {
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.FirstOrDefault() ?? throw new InvalidOperationException("No worksheet found in Excel file");

        var properties = GetTypeProperties<T>();
        var headers = worksheet.Row(1).CellsUsed().ToDictionary(cell => cell.Value.ToString(), cell => cell.Address.ColumnNumber, StringComparer.OrdinalIgnoreCase);

        int rowCount = worksheet.LastRowUsed().RowNumber();
        for (int row = 2; row <= rowCount; row++)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            var instance = Activator.CreateInstance<T>();
            foreach (var prop in properties)
            {
                if (headers.TryGetValue(prop.Name, out int col))
                {
                    var cellValue = worksheet.Cell(row, col).Value;
                    if (!worksheet.Cell(row, col).IsEmpty())
                    {
                        try
                        {
                            object convertedValue = Convert.ChangeType(cellValue, prop.PropertyType);
                            prop.SetValue(instance, convertedValue);
                        }
                        catch
                        {
                            
                        }
                    }
                }
            }
            yield return instance;
        }
    }

    private static PropertyInfo[] GetTypeProperties<T>() => typeof(T).GetProperties();
}
