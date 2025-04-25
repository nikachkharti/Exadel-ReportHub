using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReportHub.Identity.Domain.Entities;

public class UserClient
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string? ClientId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public string UserId { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
