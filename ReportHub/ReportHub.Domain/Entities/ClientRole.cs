using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReportHub.Domain.Entities;

public class ClientRole
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }
    public bool IsDeleted { get; set; } = false;
}
