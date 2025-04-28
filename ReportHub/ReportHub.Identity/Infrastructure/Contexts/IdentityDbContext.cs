using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ReportHub.Identity.Configurations;
using ReportHub.Identity.Domain.Entities;

namespace ReportHub.Identity.Infrastructure.Contexts;

public class IdentityDbContext
{
    public IMongoDatabase Database { get; private set; }

    public IMongoCollection<User> Users { get; private set; }
    
    public IMongoCollection<UserClient> UserClientRoles { get; private set; }

    public IdentityDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        Database = client.GetDatabase(settings.Value.DatabaseName);
        Users = Database.GetCollection<User>("Users");
        UserClientRoles = Database.GetCollection<UserClient>("UserClients");
    }
}