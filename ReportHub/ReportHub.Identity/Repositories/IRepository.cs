namespace ReportHub.Identity.Repositories;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();

    Task InsertAsync(T entity);
}
