using MongoDB.Driver;
using ReportHub.Identity.Domain.Entities;
using ReportHub.Identity.Infrastructure.Contexts;
using System.Linq.Expressions;

namespace ReportHub.Identity.Infrastructure.Repositories;

public class UserClientRepository(IdentityDbContext dbContext) : IUserClientRepository
{
    public async Task<IEnumerable<UserClient>> GetAllAsync()
    {
        return await dbContext.UserClientRoles.Find(_ => true).ToListAsync();
    }

    public async Task InsertAsync(UserClient entity)
    {
        await dbContext.UserClientRoles.InsertOneAsync(entity);
    }

    public Task<UserClient> GetAsync(Expression<Func<UserClient, bool>> predicate)
    {
        return dbContext.UserClientRoles.Find(predicate).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserClient>> GetAllAsync(Expression<Func<UserClient, bool>> predicate)
    {
        return await dbContext.UserClientRoles.Find(predicate).ToListAsync();
    }
}
