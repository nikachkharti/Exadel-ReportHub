using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReportHub.Domain.Entities
{
    public class Client
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}