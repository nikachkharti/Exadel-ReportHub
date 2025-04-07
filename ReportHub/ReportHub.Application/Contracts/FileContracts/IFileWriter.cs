namespace ReportHub.Application.Contracts.FileContracts;

public interface IFileWriter
{
    Task<Stream> WriteAllAsync<T>(IEnumerable<T> datas, CancellationToken token);
}
