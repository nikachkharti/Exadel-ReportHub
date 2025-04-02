using AspNetCore.Identity.Mongo.Model;
using MongoDbGenericRepository.Attributes;

namespace ReportHub.Identity.Models;

[CollectionName("Users")]
public class User : MongoUser<string>
{
}