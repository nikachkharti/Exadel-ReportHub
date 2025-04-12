using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ReportHub.Domain.Entities
{
    public class Client
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ClientId { get; set; }
        public string Name { get; set; }
        public List<string> ItemIds { get; set; }
    }
}
