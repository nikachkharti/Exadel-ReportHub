using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ReportHub.Domain.Entities
{
    public class Client : SoftDeletion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
    }
}
