namespace ReportHub.Application.Contracts.FileContracts;

public interface IFileWriter
{
    Task WriteAllAsync<T>(IEnumerable<T> datas, CancellationToken token);
}
