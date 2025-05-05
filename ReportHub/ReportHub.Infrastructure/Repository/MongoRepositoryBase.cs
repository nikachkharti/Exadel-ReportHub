using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq.Expressions;
using ReportHub.Infrastructure.Configurations;
using ReportHub.Application.Contracts.RepositoryContracts;
using MongoDB.Bson;
using ReportHub.Domain.Entities;

namespace ReportHub.Infrastructure.Repository
{
    public class MongoRepositoryBase<T> : IMongoRepositoryBase<T> where T : SoftDeletion
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepositoryBase(IOptions<MongoDbSettings> options, string collectionName)
        {
            var settings = options.Value;
            var client = new MongoClient(settings.ConnectionString);
            _mongoDatabase = client.GetDatabase(settings.DatabaseName);
            _collection = _mongoDatabase.GetCollection<T>(collectionName);
            EnsureIsDeletedFieldExistsAsync().Wait();
            EnsureEcbSupportFieldExistAsync().Wait();
        }
        private readonly IMongoDatabase _mongoDatabase;

        public async Task EnsureIsDeletedFieldExistsAsync()
        {
            var names = await _mongoDatabase.ListCollectionNames().ToListAsync();

            foreach (var name in names)
            {
                var col = _mongoDatabase.GetCollection<BsonDocument>(name);
                var update = Builders<BsonDocument>.Update.Set("IsDeleted", false);
                var filter = Builders<BsonDocument>.Filter.Exists("IsDeleted", false);
                await col.UpdateManyAsync(filter, update);
            }
        }

        public async Task EnsureEcbSupportFieldExistAsync()
        {
            var col = _mongoDatabase.GetCollection<BsonDocument>("Currency");
            var update = Builders<BsonDocument>.Update.Set("EcbSupport", false);
            var filter = Builders<BsonDocument>.Filter.Exists("EcbSupport", false);
            await col.UpdateManyAsync(filter, update);
        }

        #region GET

        public async Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default) =>
            await _collection
            .Find(_ => true)
            .ToListAsync(cancellationToken);

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, object>> sortBy, bool ascending = true, CancellationToken cancellationToken = default)
        {
            var sortDefinition = ascending
                ? Builders<T>.Sort.Ascending(sortBy)
                : Builders<T>.Sort.Descending(sortBy);

            return await _collection
            .Find(_ => true)
            .Sort(sortDefinition)
            .ToListAsync(cancellationToken);
        }


        public async Task<IEnumerable<T>> GetAll(int pageNumber, int pageSize, CancellationToken cancellationToken = default) =>
            await _collection
            .Find(_ => true)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        public async Task<IEnumerable<T>> GetAll(int pageNumber, int pageSize, Expression<Func<T, object>> sortBy, bool ascending = true, CancellationToken cancellationToken = default)
        {
            var sortDefinition = ascending
                ? Builders<T>.Sort.Ascending(sortBy)
                : Builders<T>.Sort.Descending(sortBy);

            return await _collection
            .Find(_ => true)
            .Sort(sortDefinition)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
        }


        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default) =>
            await _collection
            .Find(filter)
            .ToListAsync(cancellationToken);

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter, Expression<Func<T, object>> sortBy, bool ascending = true, CancellationToken cancellationToken = default)
        {
            var sortDefinition = ascending
                ? Builders<T>.Sort.Ascending(sortBy)
                : Builders<T>.Sort.Descending(sortBy);

            return await _collection
            .Find(filter)
            .Sort(sortDefinition)
            .ToListAsync(cancellationToken);
        }


        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default) =>
            await _collection
            .Find(filter)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter, int pageNumber, int pageSize, Expression<Func<T, object>> sortBy, bool ascending = true, CancellationToken cancellationToken = default)
        {
            var sortDefinition = ascending
                ? Builders<T>.Sort.Ascending(sortBy)
                : Builders<T>.Sort.Descending(sortBy);

            return await _collection
            .Find(filter)
            .Sort(sortDefinition)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAll(FilterDefinition<T> filter, int pageNumber, int pageSize, Expression<Func<T, object>> sortBy, bool ascending = true, CancellationToken cancellationToken = default)
        {
            var sortDefinition = ascending
                ? Builders<T>.Sort.Ascending(sortBy)
                : Builders<T>.Sort.Descending(sortBy);

            return await _collection.Find(filter)
                .Sort(sortDefinition)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default) =>
            await _collection
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        #endregion


        #region DELETE

        public async Task Delete(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            var update = Builders<T>
                    .Update
                    .Set(x => x.IsDeleted, true);
            await _collection.UpdateOneAsync(
                    filter,
                    update,
                    cancellationToken: cancellationToken
                    );
        }

        public async Task DeleteMultiple(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            var update = Builders<T>.Update.Set(x => x.IsDeleted, true);

            await _collection.UpdateManyAsync(
                filter,
                update,
                cancellationToken: cancellationToken
            );
        }



        #endregion


        #region INSERT

        //TODO implement override with InsertOneOptions
        public async Task Insert(T document, CancellationToken cancellationToken = default) =>
            await _collection
            .InsertOneAsync(document, options: null, cancellationToken);

        //TODO implement override with InsertManyOptions
        public async Task InsertMultiple(IEnumerable<T> documents, CancellationToken cancellationToken = default) =>
            await _collection
            .InsertManyAsync(documents, options: null, cancellationToken);

        #endregion


        #region UPDATE

        //TODO implement override with UpdateOptions
        public async Task UpdateSingleField(
        Expression<Func<T, bool>> filterExpression,
        Expression<Func<T, object>> field,
        object value,
        CancellationToken cancellationToken = default)
        {
            var update = Builders<T>.Update.Set(field, value);
            await _collection.UpdateOneAsync(filterExpression, update, options: null, cancellationToken);
        }

        //TODO implement override with UpdateOptions
        public async Task UpdateMultipleFields(
            Expression<Func<T, bool>> filterExpression,
            Dictionary<Expression<Func<T, object>>,
            object> updates,
            CancellationToken cancellation = default)
        {
            var updateDefinitions = updates.Select(update => Builders<T>.Update.Set(update.Key, update.Value)).ToList();
            var combinedUpdate = Builders<T>.Update.Combine(updateDefinitions);
            await _collection.UpdateOneAsync(filterExpression, combinedUpdate, options: null, cancellation);
        }


        //TODO implement override with UpdateOptions
        public async Task UpdateSingleDocument(
            Expression<Func<T, bool>> filterExpression,
            T updatedDocument,
            bool isUpsert = true,
            CancellationToken cancellationToken = default) =>
                await _collection
                .ReplaceOneAsync(
                    filterExpression,
                    updatedDocument,
                    new ReplaceOptions { IsUpsert = isUpsert },
                    cancellationToken
                );


        //TODO implement override with UpdateOptions
        public async Task UpdateMultipleDocuments(
            Expression<Func<T, bool>> filterExpression,
            Dictionary<Expression<Func<T, object>>, object> updates,
            CancellationToken cancellationToken = default)
        {
            var updateDefinition = Builders<T>.Update.Combine(
                updates.Select(update =>
                    Builders<T>.Update.Set(update.Key, update.Value)
                )
            );

            await _collection.UpdateManyAsync(filterExpression, updateDefinition, options: null, cancellationToken);
        }


        #endregion
    }
}