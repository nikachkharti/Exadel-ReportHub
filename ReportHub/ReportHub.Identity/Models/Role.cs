using AspNetCore.Identity.Mongo.Model;
using MongoDbGenericRepository.Attributes;

namespace ReportHub.Identity.Models;

[CollectionName("Roles")]
public class Role : MongoRole<string>
{
}