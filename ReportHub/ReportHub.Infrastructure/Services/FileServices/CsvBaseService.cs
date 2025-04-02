using CsvHelper;
using System.Reflection;

namespace ReportHub.Infrastructure.Services.FileServices;

public abstract class CsvBaseService
{
    private protected static int GetAllExistPropertyInCsv(CsvReader reader, PropertyInfo[] properties)
    {
        var count = 0;
        for (var i = 0; i < properties.Length; i++)
        {
            var prop = properties[i];
            var isField = reader.TryGetField(prop.PropertyType, prop.Name, out object field);
            if (!isField)
                break;
            count++;
        }

        return count;
    }

    private protected PropertyInfo[] GetTypeProperties<T>() => typeof(T).GetProperties();
}
