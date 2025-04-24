using MongoDB.Driver;
using ReportHub.Identity.Contexts;
using ReportHub.Identity.Models;
using System.Linq.Expressions;

namespace ReportHub.Identity.Repositories;

public class UserClientRoleRepository(IdentityDbContext dbContext) : IUserClientRoleRepository
{
    public async Task<IEnumerable<UserClientRole>> GetAllAsync()
    {
        return await dbContext.UserClientRoles.Find(_ => true).ToListAsync();
    }

    public async Task InsertAsync(UserClientRole entity)
    {
        await dbContext.UserClientRoles.InsertOneAsync(entity);
    }

    public Task<UserClientRole> GetAsync(Expression<Func<UserClientRole, bool>> predicate)
    {
        return dbContext.UserClientRoles.Find(predicate).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserClientRole>> GetAllAsync(Expression<Func<UserClientRole, bool>> predicate)
    {
        return await dbContext.UserClientRoles.Find(predicate).ToListAsync();
    }
}
