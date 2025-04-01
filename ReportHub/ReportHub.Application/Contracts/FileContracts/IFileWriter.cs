using System.Collections.Generic;

namespace ReportHub.Application.Contracts.FileContracts;

public interface IFileWriter
{
    Task<Stream> WriteAllAsync<T>(IEnumerable<T> datas, IReadOnlyDictionary<string, object> statistics,CancellationToken token);
}
