using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReportHub.Identity.Models;

public class UserClientRole
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ClientId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public string UserId { get; set; } = string.Empty;

    public string RoleId { get; set; } = string.Empty;
}
