using ReportHub.Domain.Entities;

namespace ReportHub.Application.Contracts.FileContracts;

public interface IFileWriter
{
    /// <summary>
    /// Takes collection of data and statistics and writes it to a stream
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="datas"></param>
    /// <param name="statistics"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<Stream> WriteAllAsync<T>(IEnumerable<T> datas, IReadOnlyDictionary<string, object> statistics,CancellationToken token);
    Task<Stream> WriteInvoiceAsync(Invoice invoices, IReadOnlyDictionary<string, object> statistics, CancellationToken token);
}
