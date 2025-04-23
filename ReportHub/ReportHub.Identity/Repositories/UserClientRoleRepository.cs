using MongoDB.Driver;
using ReportHub.Identity.Contexts;
using ReportHub.Identity.Models;

namespace ReportHub.Identity.Repositories;

public class UserClientRoleRepository(IdentityDbContext dbContext) : IRepository<UserClientRole>
{
    public async Task<IEnumerable<UserClientRole>> GetAllAsync()
    {
        return await dbContext.UserClientRoles.Find(_ => true).ToListAsync();
    }

    public async Task InsertAsync(UserClientRole entity)
    {
        await dbContext.UserClientRoles.InsertOneAsync(entity);
    }
}
