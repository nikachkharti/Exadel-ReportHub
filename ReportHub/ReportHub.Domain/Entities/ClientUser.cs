using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReportHub.Domain.Entities;

public class ClientUser : SoftDeletion
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string ClientId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public string UserId { get; set; }

    public string Role { get; set; }
}
