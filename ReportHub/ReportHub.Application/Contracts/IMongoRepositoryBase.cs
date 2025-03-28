using System.Linq.Expressions;

namespace ReportHub.Application.Contracts
{
    public interface IMongoRepositoryBase<T> where T : class
    {
        /// <summary>
        /// Get all documents
        /// </summary>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task IEnumerable T</returns>
        Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all documents with sorting
        /// </summary>
        /// <param name="sortBy">Sort by property</param>
        /// <param name="ascending">Is ascending</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task IEnumerable T</returns>
        Task<IEnumerable<T>> GetAll(Expression<Func<T, object>> sortBy, bool ascending = true, CancellationToken cancellationToken = default);


        /// <summary>
        /// Get all documents with pagination
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task IEnumerable T</returns>
        Task<IEnumerable<T>> GetAll(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all documents with pagination and sorting
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Sort by property</param>
        /// <param name="ascending">Is ascending</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task IEnumerable T</returns>
        Task<IEnumerable<T>> GetAll(int pageNumber, int pageSize, Expression<Func<T, object>> sortBy, bool ascending = true, CancellationToken cancellationToken = default);



        /// <summary>
        /// Get filtered documents
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task IEnumerable T</returns>
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get filter documents with sorting
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="sortBy">Sort by property</param>
        /// <param name="ascending">Is asending</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task IEnumerable T</returns>
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter, Expression<Func<T, object>> sortBy, bool ascending = true, CancellationToken cancellationToken = default);



        /// <summary>
        /// Get filtered documents with pagination
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task IEnumerable T</returns>
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get filtered documents with pagination and sorting
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Sort by property</param>
        /// <param name="ascending">Is ascending</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task IEnumerable T</returns>
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter, int pageNumber, int pageSize, Expression<Func<T, object>> sortBy, bool ascending = true, CancellationToken cancellationToken = default);



        /// <summary>
        /// Get single document with filter
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task T</returns>
        Task<T> Get(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete single document with filter
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task</returns>
        Task Delete(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete multiple documents with filter
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task</returns>
        Task DeleteMultiple(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Insert a single document
        /// </summary>
        /// <param name="document">Single document to insert</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task</returns>
        /// <returns>Task</returns>
        Task Insert(T document, CancellationToken cancellationToken = default);

        /// <summary>
        /// Insert multiple documents
        /// </summary>
        /// <param name="documents">Multiple documents to insert</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task</returns>
        Task InsertMultiple(IEnumerable<T> documents, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update single field with filter
        /// </summary>
        /// <param name="filterExpression">Filter document</param>
        /// <param name="field">Filter field to update</param>
        /// <param name="value">Updated value of field</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task</returns>
        Task UpdateSingleField(Expression<Func<T, bool>> filterExpression, Expression<Func<T, object>> field, object value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update multiple fields with filter
        /// </summary>
        /// <param name="filterExpression">Filter document</param>
        /// <param name="updates">Dictrionary of updatable filters with updated values</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task</returns>
        Task UpdateMultipleFields(Expression<Func<T, bool>> filterExpression, Dictionary<Expression<Func<T, object>>, object> updates, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update whole single document with filter and upsertion flag
        /// </summary>
        /// <param name="filterExpression">Filter document</param>
        /// <param name="updatedDocument">Updated documnet</param>
        /// <param name="isUpsert">Insert updated value if it not exists</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task</returns>
        Task UpdateSingleDocument(Expression<Func<T, bool>> filterExpression, T updatedDocument, bool isUpsert = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update whole multiple documents with filter
        /// </summary>
        /// <param name="filterExpression">Filter document</param>
        /// <param name="updates">Dictrionary of updatable filters with updated values</param>
        /// <param name="cancellationToken">Token for task cancel, none by default</param>
        /// <returns>Task</returns>
        Task UpdateMultipleDocuments(Expression<Func<T, bool>> filterExpression, Dictionary<Expression<Func<T, object>>, object> updates, CancellationToken cancellationToken = default);
    }
}
