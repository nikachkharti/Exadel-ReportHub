using AspNetCore.Identity.Mongo.Model;
using MongoDbGenericRepository.Attributes;

namespace ReportHub.Identity.Models;

[CollectionName("Roles")]
public sealed class Role : MongoRole<string>
{
    public Role(string roleName) :base(roleName)
    {
        Id = Guid.NewGuid().ToString();   
    }
}