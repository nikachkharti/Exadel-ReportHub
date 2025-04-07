namespace ReportHub.Application.Contracts.FileContracts;

public interface IFileReader
{
    Task<IEnumerable<T>> ReadAllAsync<T>(Stream stream, CancellationToken cancellationToken) where T : class;
}
