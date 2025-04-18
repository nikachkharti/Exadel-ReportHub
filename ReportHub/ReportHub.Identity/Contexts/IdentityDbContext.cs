using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ReportHub.Identity.Configurations;
using ReportHub.Identity.Models;

namespace ReportHub.Identity.Contexts;

public class IdentityDbContext
{
    public IMongoDatabase Database { get; private set; }
    public IMongoCollection<User> Users { get; private set; }
    
    public IdentityDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        Database = client.GetDatabase(settings.Value.DatabaseName);
        Users = Database.GetCollection<User>("Users");
    }
}