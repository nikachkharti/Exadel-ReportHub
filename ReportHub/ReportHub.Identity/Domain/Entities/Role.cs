using AspNetCore.Identity.Mongo.Model;
using MongoDbGenericRepository.Attributes;

namespace ReportHub.Identity.Domain.Entities;

[CollectionName("Roles")]
public class Role : MongoRole<string>
{
    public Role(string roleName) : base(roleName)
    {
        Id = Guid.NewGuid().ToString();
    }
}
