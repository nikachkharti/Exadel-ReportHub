using System.Linq.Expressions;

namespace ReportHub.Identity.Repositories;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();

    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);


    Task<T> GetAsync(Expression<Func<T, bool>> predicate);

    Task InsertAsync(T entity);
}
