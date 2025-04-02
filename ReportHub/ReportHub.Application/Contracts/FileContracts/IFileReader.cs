namespace ReportHub.Application.Contracts.FileContracts;

public interface IFileReader
{
    /// <summary>
    /// Takes opened stream as parameter and read all data and return it as IEnumberable collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<T>> ReadAllAsync<T>(Stream stream, CancellationToken cancellationToken) where T : class;
}
