using OfficeOpenXml;
using ReportHub.Application.Contracts.FileContracts;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;


namespace ReportHub.Infrastructure.Services.FileServices
{
    public class ExcelService : IExcelService
    {
        public async IAsyncEnumerable<T> ReadAllAsync<T>(Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken) where T : class
        {
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
                yield break;

            var properties = typeof(T).GetProperties();

            if (worksheet.Dimension == null)
                yield break;

            int rowCount = worksheet.Dimension.Rows;
            int colCount = worksheet.Dimension.Columns;

            var headerMap = new Dictionary<int, PropertyInfo>(colCount);
            for (int col = 1; col <= colCount; col++)
            {
                var headerValue = worksheet.Cells[1, col].Text?.Trim();
                if (string.IsNullOrEmpty(headerValue))
                    continue;

                var property = properties.FirstOrDefault(p =>
                    p.Name.Equals(headerValue, StringComparison.OrdinalIgnoreCase));

                if (property != null)
                    headerMap[col] = property;
            }

            if (!headerMap.Any())
                yield break;

            for (int row = 2; row <= rowCount; row++)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                var obj = Activator.CreateInstance<T>();
                bool hasData = false;

                foreach (var (col, prop) in headerMap)
                {
                    var cell = worksheet.Cells[row, col];
                    var cellValue = cell.Value;

                    if (cellValue != null)
                    {
                        try
                        {
                            object convertedValue;

                            if (prop.PropertyType == typeof(DateTime) && cell.Style.Numberformat.NumFmtID > 0)
                            {
                                if (cellValue is double dateDouble)
                                    convertedValue = DateTime.FromOADate(dateDouble);
                                else
                                    convertedValue = Convert.ChangeType(cellValue, prop.PropertyType, CultureInfo.InvariantCulture);
                            }
                            else if (prop.PropertyType.IsEnum && cellValue is string enumString)
                            {
                                convertedValue = Enum.Parse(prop.PropertyType, enumString, true);
                            }
                            else if (prop.PropertyType == typeof(bool) && (cellValue is string boolString))
                            {
                                convertedValue = boolString.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                                                boolString.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                                                boolString.Equals("1", StringComparison.OrdinalIgnoreCase);
                            }
                            else
                            {
                                convertedValue = Convert.ChangeType(cellValue, prop.PropertyType, CultureInfo.InvariantCulture);
                            }

                            prop.SetValue(obj, convertedValue);
                            hasData = true;
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }

                if (hasData)
                {
                    // For async yielding, we can add a delay or some work here if needed
                    await Task.Yield(); // This allows other tasks to execute
                    yield return obj;
                }
            }
        }

        public async Task<Stream> WriteAllAsync<T>(IEnumerable<T> datas, IReadOnlyDictionary<string, object> statistics, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var memoryStream = new MemoryStream();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Data");
                await WriteDataToWorksheetAsync(datas, worksheet, token);

                if (statistics != null && statistics.Any())
                {
                    var statsSheet = package.Workbook.Worksheets.Add("Statistics");
                    WriteStatistics(statistics, statsSheet);
                }

                ApplyWorkbookFormatting(package, typeof(T));

                await package.SaveAsAsync(memoryStream, token);
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        private async Task WriteDataToWorksheetAsync<T>(IEnumerable<T> data, ExcelWorksheet worksheet, CancellationToken token)
        {
            var properties = typeof(T).GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = properties[i].Name;

                using (var range = worksheet.Cells[1, i + 1])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
            }

            if (data != null)
            {
                int row = 2;
                foreach (var item in data)
                {
                    if (row % 100 == 0)
                    {
                        token.ThrowIfCancellationRequested();
                        await Task.Yield(); // Allow other tasks to execute periodically
                    }

                    for (int col = 0; col < properties.Length; col++)
                    {
                        var value = properties[col].GetValue(item);
                        worksheet.Cells[row, col + 1].Value = value;

                        if (value is DateTime)
                            worksheet.Cells[row, col + 1].Style.Numberformat.Format = "yyyy-mm-dd";
                        else if (value is decimal || value is double || value is float)
                            worksheet.Cells[row, col + 1].Style.Numberformat.Format = "#,##0.00";
                    }
                    row++;
                }

                for (int col = 1; col <= properties.Length; col++)
                {
                    worksheet.Column(col).AutoFit();
                }
            }
        }

        private static void WriteStatistics(IReadOnlyDictionary<string, object> statistics, ExcelWorksheet worksheet)
        {
            if (statistics == null)
                return;

            worksheet.Cells[1, 1].Value = "Metric";
            worksheet.Cells[1, 2].Value = "Value";

            using (var headerRange = worksheet.Cells[1, 1, 1, 2])
            {
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            int row = 2;
            foreach (var (key, value) in statistics)
            {
                worksheet.Cells[row, 1].Value = key;
                worksheet.Cells[row, 2].Value = value;

                if (value is DateTime)
                    worksheet.Cells[row, 2].Style.Numberformat.Format = "yyyy-mm-dd";
                else if (value is decimal || value is double || value is float)
                    worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0.00";
                else if (value is int || value is long)
                    worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
                else if (value is bool boolVal)
                    worksheet.Cells[row, 2].Value = boolVal ? "Yes" : "No";

                row++;
            }

            worksheet.Column(1).AutoFit();
            worksheet.Column(2).AutoFit();
        }

        private void ApplyWorkbookFormatting(ExcelPackage package, Type dataType)
        {
            foreach (var worksheet in package.Workbook.Worksheets)
            {
                worksheet.View.FreezePanes(2, 1);

                worksheet.PrinterSettings.FitToPage = true;
                worksheet.PrinterSettings.FitToWidth = 1;
                worksheet.PrinterSettings.FitToHeight = 0;
                worksheet.PrinterSettings.RepeatRows = new ExcelAddress("1:1");
            }
        }
    }
}