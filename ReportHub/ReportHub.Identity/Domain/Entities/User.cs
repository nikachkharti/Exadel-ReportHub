using AspNetCore.Identity.Mongo.Model;
using MongoDbGenericRepository.Attributes;

namespace ReportHub.Identity.Domain.Entities;

[CollectionName("Users")]
public sealed class User : MongoUser<string>
{
    public User()
    {
        Id = Guid.NewGuid().ToString();
    }
}